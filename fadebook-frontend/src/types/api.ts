export interface AppointmentDto {
  appointmentId: number;
  status: string;
  customerId: number;
  serviceId: number;
  barberId: number;
  appointmentDate: string;
}

export interface BarberDto {
  barberId: number;
  username: string;
  name: string;
  specialty: string;
  contactInfo: string;
}

export interface CustomerDto {
  customerId: number;
  username: string;
  name: string;
  contactInfo: string;
}

export interface ServiceDto {
  serviceId: number;
  serviceName: string;
  servicePrice: number;
}

export interface AppointmentRequestDto {
  customer: CustomerDto;
  appointment: AppointmentDto;
}

export interface CreateAppointmentDto {
  status: string;
  customerId: number;
  serviceId: number;
  barberId: number;
  appointmentDate: string;
}

export interface CreateBarberDto {
  username: string;
  name: string;
  specialty: string;
  contactInfo: string;
  serviceIds: number[];
}

export interface CreateCustomerDto {
  username: string;
  name: string;
  contactInfo: string;
}
