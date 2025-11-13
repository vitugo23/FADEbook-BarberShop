import { axiosInstance } from '../axios';
import type { AppointmentDto, CreateAppointmentDto } from '@/types/api';

export const appointmentsApi = {
  // GET: api/appointment/{id}
  getById: async (id: number): Promise<AppointmentDto> => {
    const { data } = await axiosInstance.get(`/api/appointment/${id}`);
    return data;
  },

  // POST: api/appointment
  create: async (appointment: CreateAppointmentDto): Promise<AppointmentDto> => {
    const { data } = await axiosInstance.post('/api/appointment', appointment);
    return data;
  },

  // PUT: api/appointment/{id}
  update: async (id: number, appointment: CreateAppointmentDto): Promise<AppointmentDto> => {
    const { data } = await axiosInstance.put(`/api/appointment/${id}`, appointment);
    return data;
  },

  // DELETE: api/appointment/{id}
  delete: async (id: number): Promise<void> => {
    await axiosInstance.delete(`/api/appointment/${id}`);
  },

  // GET: api/appointment/by-date?date=2025-01-01
  getByDate: async (date: string): Promise<AppointmentDto[]> => {
    const { data } = await axiosInstance.get('/api/appointment/by-date', {
      params: { date },
    });
    return data;
  },

  // GET: api/appointment/by-username/{username}
  getByUsername: async (username: string): Promise<AppointmentDto[]> => {
    const { data } = await axiosInstance.get(`/api/appointment/by-username/${username}`);
    return data;
  },
};
