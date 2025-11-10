import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { BarberInfo, PortfolioItem, ServiceType, Appointment } from '../../models/models';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit {
  activeTab: 'info' | 'portfolio' | 'services' | 'appointments' = 'info';
  
  barberInfo?: BarberInfo;
  portfolioItems: PortfolioItem[] = [];
  serviceTypes: ServiceType[] = [];
  appointments: Appointment[] = [];
  
  editingInfo = false;
  editedInfo: Partial<BarberInfo> = {};
  
  newPortfolio: Partial<PortfolioItem> = {};
  newService: Partial<ServiceType> = {};
  
  loading = false;
  message?: { type: 'success' | 'error', text: string };

  constructor(private apiService: ApiService) { }

  ngOnInit(): void {
    this.loadAllData();
  }

  loadAllData(): void {
    this.loadBarberInfo();
    this.loadPortfolio();
    this.loadServiceTypes();
    this.loadAppointments();
  }

  loadBarberInfo(): void {
    this.apiService.getBarberInfo().subscribe({
      next: (data) => {
        this.barberInfo = data;
      },
      error: (err) => console.error('Error loading barber info:', err)
    });
  }

  loadPortfolio(): void {
    this.apiService.getPortfolio().subscribe({
      next: (data) => {
        this.portfolioItems = data;
      },
      error: (err) => console.error('Error loading portfolio:', err)
    });
  }

  loadServiceTypes(): void {
    this.apiService.getServiceTypes().subscribe({
      next: (data) => {
        this.serviceTypes = data;
      },
      error: (err) => console.error('Error loading service types:', err)
    });
  }

  loadAppointments(): void {
    const startDate = new Date();
    startDate.setDate(startDate.getDate() - 7);
    const endDate = new Date();
    endDate.setDate(endDate.getDate() + 30);
    
    this.apiService.getAppointments(startDate, endDate).subscribe({
      next: (data) => {
        this.appointments = data;
      },
      error: (err) => console.error('Error loading appointments:', err)
    });
  }

  startEditingInfo(): void {
    if (this.barberInfo) {
      this.editedInfo = { ...this.barberInfo };
      this.editingInfo = true;
    }
  }

  saveBarberInfo(): void {
    this.loading = true;
    this.apiService.updateBarberInfo(this.editedInfo).subscribe({
      next: () => {
        this.loading = false;
        this.editingInfo = false;
        this.loadBarberInfo();
        this.showMessage('success', 'Information updated successfully');
      },
      error: (err) => {
        console.error('Error updating info:', err);
        this.loading = false;
        this.showMessage('error', 'Failed to update information');
      }
    });
  }

  addPortfolioItem(): void {
    if (!this.newPortfolio.title || !this.newPortfolio.imageUrl) {
      this.showMessage('error', 'Title and image URL are required');
      return;
    }

    this.loading = true;
    this.apiService.createPortfolioItem(this.newPortfolio).subscribe({
      next: () => {
        this.loading = false;
        this.newPortfolio = {};
        this.loadPortfolio();
        this.showMessage('success', 'Portfolio item added successfully');
      },
      error: (err) => {
        console.error('Error adding portfolio item:', err);
        this.loading = false;
        this.showMessage('error', 'Failed to add portfolio item');
      }
    });
  }

  deletePortfolioItem(id: number): void {
    if (!confirm('Are you sure you want to delete this item?')) return;
    
    this.apiService.deletePortfolioItem(id).subscribe({
      next: () => {
        this.loadPortfolio();
        this.showMessage('success', 'Portfolio item deleted');
      },
      error: (err) => {
        console.error('Error deleting portfolio item:', err);
        this.showMessage('error', 'Failed to delete portfolio item');
      }
    });
  }

  addServiceType(): void {
    if (!this.newService.name || !this.newService.price || !this.newService.durationMinutes) {
      this.showMessage('error', 'Name, price, and duration are required');
      return;
    }

    this.loading = true;
    this.apiService.createServiceType(this.newService).subscribe({
      next: () => {
        this.loading = false;
        this.newService = {};
        this.loadServiceTypes();
        this.showMessage('success', 'Service added successfully');
      },
      error: (err) => {
        console.error('Error adding service:', err);
        this.loading = false;
        this.showMessage('error', 'Failed to add service');
      }
    });
  }

  deleteServiceType(id: number): void {
    if (!confirm('Are you sure you want to deactivate this service?')) return;
    
    this.apiService.deleteServiceType(id).subscribe({
      next: () => {
        this.loadServiceTypes();
        this.showMessage('success', 'Service deactivated');
      },
      error: (err) => {
        console.error('Error deleting service:', err);
        this.showMessage('error', 'Failed to deactivate service');
      }
    });
  }

  updateAppointmentStatus(id: number, status: string): void {
    this.apiService.updateAppointment(id, { status }).subscribe({
      next: () => {
        this.loadAppointments();
        this.showMessage('success', 'Appointment status updated');
      },
      error: (err) => {
        console.error('Error updating appointment:', err);
        this.showMessage('error', 'Failed to update appointment');
      }
    });
  }

  deleteAppointment(id: number): void {
    if (!confirm('Are you sure you want to delete this appointment?')) return;
    
    this.apiService.deleteAppointment(id).subscribe({
      next: () => {
        this.loadAppointments();
        this.showMessage('success', 'Appointment deleted');
      },
      error: (err) => {
        console.error('Error deleting appointment:', err);
        this.showMessage('error', 'Failed to delete appointment');
      }
    });
  }

  showMessage(type: 'success' | 'error', text: string): void {
    this.message = { type, text };
    setTimeout(() => {
      this.message = undefined;
    }, 5000);
  }

  formatDate(date: Date): string {
    return new Date(date).toLocaleDateString() + ' ' + new Date(date).toLocaleTimeString();
  }
}
