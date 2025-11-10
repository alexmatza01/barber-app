export interface BarberInfo {
  id: number;
  name: string;
  bio: string;
  profileImage?: string;
  phone?: string;
  email?: string;
  address?: string;
  googleCalendarId?: string;
  createdAt: Date;
  updatedAt: Date;
}

export interface PortfolioItem {
  id: number;
  title: string;
  description?: string;
  imageUrl: string;
  order: number;
  createdAt: Date;
}

export interface ServiceType {
  id: number;
  name: string;
  description?: string;
  price: number;
  durationMinutes: number;
  isActive: boolean;
}

export interface Appointment {
  id: number;
  customerName: string;
  customerEmail: string;
  customerPhone?: string;
  startTime: Date;
  endTime: Date;
  serviceType: string;
  notes?: string;
  status: string;
  createdAt: Date;
}

export interface CreateAppointmentDto {
  customerName: string;
  customerEmail: string;
  customerPhone?: string;
  startTime: Date;
  serviceTypeId: number;
  notes?: string;
}

export interface AvailableSlot {
  startTime: Date;
  endTime: Date;
}
