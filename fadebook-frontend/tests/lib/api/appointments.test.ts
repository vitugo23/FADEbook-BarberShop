import { describe, it, expect, vi, beforeEach } from 'vitest';
import { appointmentsApi } from '@/lib/api/appointments';
import { axiosInstance } from '@/lib/axios';
import type { AppointmentDto, CreateAppointmentDto } from '@/types/api';

vi.mock('@/lib/axios');

describe('appointmentsApi', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe('getById', () => {
    it('should fetch appointment by id', async () => {
      const mockAppointment: AppointmentDto = {
        appointmentId: 1,
        status: 'Pending',
        customerId: 1,
        serviceId: 1,
        barberId: 1,
        appointmentDate: '2025-10-15T10:00:00Z',
      };

      vi.mocked(axiosInstance.get).mockResolvedValue({ data: mockAppointment });

      const result = await appointmentsApi.getById(1);

      expect(axiosInstance.get).toHaveBeenCalledWith('/api/appointment/1');
      expect(result).toEqual(mockAppointment);
    });
  });

  describe('create', () => {
    it('should create a new appointment', async () => {
      const newAppointment: CreateAppointmentDto = {
        status: 'Pending',
        customerId: 1,
        serviceId: 1,
        barberId: 1,
        appointmentDate: '2025-10-15T10:00:00Z',
      };

      const mockResponse: AppointmentDto = {
        appointmentId: 1,
        ...newAppointment,
      };

      vi.mocked(axiosInstance.post).mockResolvedValue({ data: mockResponse });

      const result = await appointmentsApi.create(newAppointment);

      expect(axiosInstance.post).toHaveBeenCalledWith('/api/appointment', newAppointment);
      expect(result).toEqual(mockResponse);
    });
  });

  describe('update', () => {
    it('should update an existing appointment', async () => {
      const updatedAppointment: CreateAppointmentDto = {
        status: 'Completed',
        customerId: 1,
        serviceId: 1,
        barberId: 1,
        appointmentDate: '2025-10-15T10:00:00Z',
      };

      const mockResponse: AppointmentDto = {
        appointmentId: 1,
        ...updatedAppointment,
      };

      vi.mocked(axiosInstance.put).mockResolvedValue({ data: mockResponse });

      const result = await appointmentsApi.update(1, updatedAppointment);

      expect(axiosInstance.put).toHaveBeenCalledWith('/api/appointment/1', updatedAppointment);
      expect(result).toEqual(mockResponse);
    });
  });

  describe('delete', () => {
    it('should delete an appointment', async () => {
      vi.mocked(axiosInstance.delete).mockResolvedValue({ data: undefined });

      await appointmentsApi.delete(1);

      expect(axiosInstance.delete).toHaveBeenCalledWith('/api/appointment/1');
    });
  });

  describe('getByDate', () => {
    it('should fetch appointments by date', async () => {
      const mockAppointments: AppointmentDto[] = [
        {
          appointmentId: 1,
          status: 'Pending',
          customerId: 1,
          serviceId: 1,
          barberId: 1,
          appointmentDate: '2025-10-15T10:00:00Z',
        },
        {
          appointmentId: 2,
          status: 'Confirmed',
          customerId: 2,
          serviceId: 2,
          barberId: 2,
          appointmentDate: '2025-10-15T14:00:00Z',
        },
      ];

      vi.mocked(axiosInstance.get).mockResolvedValue({ data: mockAppointments });

      const result = await appointmentsApi.getByDate('2025-10-15');

      expect(axiosInstance.get).toHaveBeenCalledWith('/api/appointment/by-date', {
        params: { date: '2025-10-15' },
      });
      expect(result).toEqual(mockAppointments);
    });
  });

  describe('getByUsername', () => {
    it('should fetch appointments by username', async () => {
      const mockAppointments: AppointmentDto[] = [
        {
          appointmentId: 1,
          status: 'Pending',
          customerId: 1,
          serviceId: 1,
          barberId: 1,
          appointmentDate: '2025-10-15T10:00:00Z',
        },
      ];

      vi.mocked(axiosInstance.get).mockResolvedValue({ data: mockAppointments });

      const result = await appointmentsApi.getByUsername('john_doe');

      expect(axiosInstance.get).toHaveBeenCalledWith('/api/appointment/by-username/john_doe');
      expect(result).toEqual(mockAppointments);
    });
  });
});
