import { describe, it, expect, vi, beforeEach } from 'vitest';
import { renderHook, waitFor } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { useAppointment, useAppointmentsByDate, useAppointmentsByUsername, useCreateAppointment, useUpdateAppointment, useDeleteAppointment } from '@/hooks/useAppointments';
import { appointmentsApi } from '@/lib/api';
import type { AppointmentDto, CreateAppointmentDto } from '@/types/api';

vi.mock('@/lib/api', () => ({
  appointmentsApi: {
    getById: vi.fn(),
    getByDate: vi.fn(),
    getByUsername: vi.fn(),
    create: vi.fn(),
    update: vi.fn(),
    delete: vi.fn(),
  },
}));

const createWrapper = () => {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: { retry: false },
      mutations: { retry: false },
    },
  });
  // eslint-disable-next-line react/display-name
  return ({ children }: { children: React.ReactNode }) => {
    return <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>;
  };
};

describe('useAppointments hooks', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe('useAppointment', () => {
    it('should fetch appointment by id', async () => {
      const mockAppointment: AppointmentDto = {
        appointmentId: 1,
        status: 'Pending',
        customerId: 1,
        serviceId: 1,
        barberId: 1,
        appointmentDate: '2025-10-15T10:00:00Z',
      };

      vi.mocked(appointmentsApi.getById).mockResolvedValue(mockAppointment);

      const { result } = renderHook(() => useAppointment(1), {
        wrapper: createWrapper(),
      });

      await waitFor(() => expect(result.current.isSuccess).toBe(true));

      expect(appointmentsApi.getById).toHaveBeenCalledWith(1);
      expect(result.current.data).toEqual(mockAppointment);
    });

    it('should not fetch when id is not provided', () => {
      const { result } = renderHook(() => useAppointment(0), {
        wrapper: createWrapper(),
      });

      expect(result.current.isFetching).toBe(false);
      expect(appointmentsApi.getById).not.toHaveBeenCalled();
    });
  });

  describe('useAppointmentsByDate', () => {
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
      ];

      vi.mocked(appointmentsApi.getByDate).mockResolvedValue(mockAppointments);

      const { result } = renderHook(() => useAppointmentsByDate('2025-10-15'), {
        wrapper: createWrapper(),
      });

      await waitFor(() => expect(result.current.isSuccess).toBe(true));

      expect(appointmentsApi.getByDate).toHaveBeenCalledWith('2025-10-15');
      expect(result.current.data).toEqual(mockAppointments);
    });
  });

  describe('useAppointmentsByUsername', () => {
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

      vi.mocked(appointmentsApi.getByUsername).mockResolvedValue(mockAppointments);

      const { result } = renderHook(() => useAppointmentsByUsername('john_doe'), {
        wrapper: createWrapper(),
      });

      await waitFor(() => expect(result.current.isSuccess).toBe(true));

      expect(appointmentsApi.getByUsername).toHaveBeenCalledWith('john_doe');
      expect(result.current.data).toEqual(mockAppointments);
    });
  });

  describe('useCreateAppointment', () => {
    it('should create appointment', async () => {
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

      vi.mocked(appointmentsApi.create).mockResolvedValue(mockResponse);

      const { result } = renderHook(() => useCreateAppointment(), {
        wrapper: createWrapper(),
      });

      result.current.mutate(newAppointment);

      await waitFor(() => expect(result.current.isSuccess).toBe(true));

      expect(appointmentsApi.create).toHaveBeenCalledWith(newAppointment);
      expect(result.current.data).toEqual(mockResponse);
    });
  });

  describe('useUpdateAppointment', () => {
    it('should update appointment', async () => {
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

      vi.mocked(appointmentsApi.update).mockResolvedValue(mockResponse);

      const { result } = renderHook(() => useUpdateAppointment(), {
        wrapper: createWrapper(),
      });

      result.current.mutate({ id: 1, appointment: updatedAppointment });

      await waitFor(() => expect(result.current.isSuccess).toBe(true));

      expect(appointmentsApi.update).toHaveBeenCalledWith(1, updatedAppointment);
      expect(result.current.data).toEqual(mockResponse);
    });
  });

  describe('useDeleteAppointment', () => {
    it('should delete appointment', async () => {
      vi.mocked(appointmentsApi.delete).mockResolvedValue(undefined);

      const { result } = renderHook(() => useDeleteAppointment(), {
        wrapper: createWrapper(),
      });

      result.current.mutate(1);

      await waitFor(() => expect(result.current.isSuccess).toBe(true));

      expect(appointmentsApi.delete).toHaveBeenCalledWith(1);
    });
  });
});
