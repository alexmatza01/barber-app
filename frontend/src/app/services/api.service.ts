import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { 
  BarberInfo, 
  PortfolioItem, 
  ServiceType, 
  Appointment, 
  CreateAppointmentDto,
  AvailableSlot 
} from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private apiUrl = 'http://localhost:5000/api';

  constructor(private http: HttpClient) { }

  // Barber Info
  getBarberInfo(): Observable<BarberInfo> {
    return this.http.get<BarberInfo>(`${this.apiUrl}/barberinfo`);
  }

  updateBarberInfo(info: Partial<BarberInfo>): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/admin/barber-info`, info);
  }

  // Portfolio
  getPortfolio(): Observable<PortfolioItem[]> {
    return this.http.get<PortfolioItem[]>(`${this.apiUrl}/portfolio`);
  }

  createPortfolioItem(item: Partial<PortfolioItem>): Observable<PortfolioItem> {
    return this.http.post<PortfolioItem>(`${this.apiUrl}/admin/portfolio`, item);
  }

  updatePortfolioItem(id: number, item: Partial<PortfolioItem>): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/admin/portfolio/${id}`, item);
  }

  deletePortfolioItem(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/admin/portfolio/${id}`);
  }

  // Service Types
  getServiceTypes(): Observable<ServiceType[]> {
    return this.http.get<ServiceType[]>(`${this.apiUrl}/servicetypes`);
  }

  createServiceType(service: Partial<ServiceType>): Observable<ServiceType> {
    return this.http.post<ServiceType>(`${this.apiUrl}/admin/service-types`, service);
  }

  updateServiceType(id: number, service: Partial<ServiceType>): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/admin/service-types/${id}`, service);
  }

  deleteServiceType(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/admin/service-types/${id}`);
  }

  // Appointments
  getAppointments(startDate?: Date, endDate?: Date): Observable<Appointment[]> {
    let params = new HttpParams();
    if (startDate) {
      params = params.set('startDate', startDate.toISOString());
    }
    if (endDate) {
      params = params.set('endDate', endDate.toISOString());
    }
    return this.http.get<Appointment[]>(`${this.apiUrl}/appointments`, { params });
  }

  createAppointment(appointment: CreateAppointmentDto): Observable<Appointment> {
    return this.http.post<Appointment>(`${this.apiUrl}/appointments`, appointment);
  }

  updateAppointment(id: number, appointment: Partial<Appointment>): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/appointments/${id}`, appointment);
  }

  deleteAppointment(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/appointments/${id}`);
  }

  getAvailableSlots(date: Date, serviceTypeId: number): Observable<AvailableSlot[]> {
    const params = new HttpParams()
      .set('date', date.toISOString())
      .set('serviceTypeId', serviceTypeId.toString());
    return this.http.get<AvailableSlot[]>(`${this.apiUrl}/appointments/available-slots`, { params });
  }
}
