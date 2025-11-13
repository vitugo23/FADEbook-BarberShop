import { axiosInstance } from '../axios';
import type { CustomerDto, CreateCustomerDto, ServiceDto, BarberDto, AppointmentDto, AppointmentRequestDto } from '@/types/api';

export const customersApi = {
  // POST: api/customerappointment
  create: async (customer: CreateCustomerDto): Promise<CustomerDto> => {
    const { data } = await axiosInstance.post('/api/customerappointment', customer);
    return data;
  },

  // GET: /customer/{id}
  getById: async (id: number): Promise<CustomerDto> => {
    const { data } = await axiosInstance.get(`/customer/${id}`);
    return data;
  },

  // GET: api/customer/services
  getServices: async (): Promise<ServiceDto[]> => {
    const { data } = await axiosInstance.get('/api/customer/services');
    return data;
  },

  // GET: api/customer/barbers-by-service/{serviceId}
  getBarbersByService: async (serviceId: number): Promise<BarberDto[]> => {
    const { data } = await axiosInstance.get(`/api/customer/barbers-by-service/${serviceId}`);
    return data;
  },

  // POST: api/customer/request-appointment
  requestAppointment: async (request: AppointmentRequestDto): Promise<AppointmentDto> => {
    const { data } = await axiosInstance.post('/api/customer/request-appointment', request);
    return data;
  },

  // GET: api/customeraccount/username-exists/{username}
  usernameExists: async (username: string): Promise<boolean> => {
    const { data } = await axiosInstance.get(`/api/customeraccount/username-exists/${encodeURIComponent(username)}`);
    return Boolean(data?.exists);
  },
};
