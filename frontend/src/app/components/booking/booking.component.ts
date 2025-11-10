import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { ServiceType, AvailableSlot, CreateAppointmentDto } from '../../models/models';

@Component({
  selector: 'app-booking',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './booking.component.html',
  styleUrls: ['./booking.component.css']
})
export class BookingComponent implements OnInit {
  serviceTypes: ServiceType[] = [];
  availableSlots: AvailableSlot[] = [];
  
  selectedServiceTypeId?: number;
  selectedDate?: Date;
  selectedSlot?: AvailableSlot;
  
  customerName = '';
  customerEmail = '';
  customerPhone = '';
  notes = '';
  
  loading = false;
  loadingSlots = false;
  error?: string;
  success = false;
  
  minDate: string;

  constructor(private apiService: ApiService) {
    const today = new Date();
    this.minDate = today.toISOString().split('T')[0];
  }

  ngOnInit(): void {
    this.loadServiceTypes();
  }

  loadServiceTypes(): void {
    this.loading = true;
    this.apiService.getServiceTypes().subscribe({
      next: (data) => {
        this.serviceTypes = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading service types:', err);
        this.error = 'Failed to load services';
        this.loading = false;
      }
    });
  }

  onServiceTypeChange(): void {
    this.availableSlots = [];
    this.selectedSlot = undefined;
    if (this.selectedDate && this.selectedServiceTypeId) {
      this.loadAvailableSlots();
    }
  }

  onDateChange(): void {
    this.availableSlots = [];
    this.selectedSlot = undefined;
    if (this.selectedDate && this.selectedServiceTypeId) {
      this.loadAvailableSlots();
    }
  }

  loadAvailableSlots(): void {
    if (!this.selectedDate || !this.selectedServiceTypeId) return;
    
    this.loadingSlots = true;
    this.apiService.getAvailableSlots(this.selectedDate, this.selectedServiceTypeId).subscribe({
      next: (data) => {
        this.availableSlots = data;
        this.loadingSlots = false;
      },
      error: (err) => {
        console.error('Error loading available slots:', err);
        this.error = 'Failed to load available time slots';
        this.loadingSlots = false;
      }
    });
  }

  selectSlot(slot: AvailableSlot): void {
    this.selectedSlot = slot;
  }

  bookAppointment(): void {
    if (!this.selectedSlot || !this.selectedServiceTypeId) {
      this.error = 'Please select a service and time slot';
      return;
    }

    if (!this.customerName || !this.customerEmail) {
      this.error = 'Please fill in your name and email';
      return;
    }

    const appointment: CreateAppointmentDto = {
      customerName: this.customerName,
      customerEmail: this.customerEmail,
      customerPhone: this.customerPhone || undefined,
      startTime: new Date(this.selectedSlot.startTime),
      serviceTypeId: this.selectedServiceTypeId,
      notes: this.notes || undefined
    };

    this.loading = true;
    this.error = undefined;

    this.apiService.createAppointment(appointment).subscribe({
      next: () => {
        this.success = true;
        this.loading = false;
        this.resetForm();
      },
      error: (err) => {
        console.error('Error creating appointment:', err);
        this.error = err.error?.title || 'Failed to book appointment';
        this.loading = false;
      }
    });
  }

  resetForm(): void {
    this.selectedServiceTypeId = undefined;
    this.selectedDate = undefined;
    this.selectedSlot = undefined;
    this.customerName = '';
    this.customerEmail = '';
    this.customerPhone = '';
    this.notes = '';
    this.availableSlots = [];
  }

  formatTime(date: Date): string {
    return new Date(date).toLocaleTimeString('en-US', { 
      hour: '2-digit', 
      minute: '2-digit' 
    });
  }
}
