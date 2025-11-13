import { axiosInstance } from '../axios';
import type { BarberDto, CreateBarberDto, ServiceDto } from '@/types/api';

export const barbersApi = {
  // GET: api/barber
  getAll: async (): Promise<BarberDto[]> => {
    const { data } = await axiosInstance.get('/api/barber');
    return data;
  },

  // GET: api/barber/{id}
  getById: async (id: number): Promise<BarberDto> => {
    const { data } = await axiosInstance.get(`/api/barber/${id}`);
    return data;
  },

  // POST: api/barber
  create: async (barber: CreateBarberDto): Promise<BarberDto> => {
    const { data } = await axiosInstance.post('/api/barber', barber);
    return data;
  },

  // PUT: api/barber/{id}
  update: async (id: number, barber: CreateBarberDto): Promise<BarberDto> => {
    const { data } = await axiosInstance.put(`/api/barber/${id}`, barber);
    return data;
  },

  // DELETE: api/barber/{id}
  delete: async (id: number): Promise<void> => {
    await axiosInstance.delete(`/api/barber/${id}`);
  },

  // GET: api/barber/{id}/services
  getBarberServices: async (id: number): Promise<ServiceDto[]> => {
    const { data } = await axiosInstance.get(`/api/barber/${id}/services`);
    return data;
  },

  // PUT: api/barber/{id}/services
  updateServices: async (id: number, serviceIds: number[]): Promise<void> => {
    await axiosInstance.put(`/api/barber/${id}/services`, serviceIds);
  },
};
