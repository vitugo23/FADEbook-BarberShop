import { describe, it, expect, vi, beforeEach } from 'vitest';
import { renderHook, waitFor } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { useCustomer, useServices, useBarbersByService, useCreateCustomer, useRequestAppointment } from '@/hooks/useCustomers';
import { customersApi } from '@/lib/api';
import type { CustomerDto, ServiceDto, BarberDto, AppointmentDto, CreateCustomerDto, AppointmentRequestDto } from '@/types/api';

vi.mock('@/lib/api', () => ({
  customersApi: {
    getById: vi.fn(),
    getServices: vi.fn(),
    getBarbersByService: vi.fn(),
    create: vi.fn(),
    requestAppointment: vi.fn(),
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

describe('useCustomers hooks', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe('useCustomer', () => {
    it('should fetch customer by id', async () => {
      const mockCustomer: CustomerDto = {
        customerId: 1,
        username: 'customer1',
        name: 'John Customer',
        contactInfo: '555-0201',
      };

      vi.mocked(customersApi.getById).mockResolvedValue(mockCustomer);

      const { result } = renderHook(() => useCustomer(1), {
        wrapper: createWrapper(),
      });

      await waitFor(() => expect(result.current.isSuccess).toBe(true));

      expect(customersApi.getById).toHaveBeenCalledWith(1);
      expect(result.current.data).toEqual(mockCustomer);
    });

    it('should not fetch when id is not provided', () => {
      const { result } = renderHook(() => useCustomer(0), {
        wrapper: createWrapper(),
      });

      expect(result.current.isFetching).toBe(false);
      expect(customersApi.getById).not.toHaveBeenCalled();
    });
  });

  describe('useServices', () => {
    it('should fetch all services', async () => {
      const mockServices: ServiceDto[] = [
        {
          serviceId: 1,
          serviceName: 'Haircut',
          servicePrice: 20,
        },
        {
          serviceId: 2,
          serviceName: 'Beard Trim',
          servicePrice: 15,
        },
      ];

      vi.mocked(customersApi.getServices).mockResolvedValue(mockServices);

      const { result } = renderHook(() => useServices(), {
        wrapper: createWrapper(),
      });

      await waitFor(() => expect(result.current.isSuccess).toBe(true));

      expect(customersApi.getServices).toHaveBeenCalled();
      expect(result.current.data).toEqual(mockServices);
    });
  });

  describe('useBarbersByService', () => {
    it('should fetch barbers by service', async () => {
      const mockBarbers: BarberDto[] = [
        {
          barberId: 1,
          username: 'barber1',
          name: 'John Barber',
          specialty: 'Classic Cuts',
          contactInfo: '555-0101',
        },
      ];

      vi.mocked(customersApi.getBarbersByService).mockResolvedValue(mockBarbers);

      const { result } = renderHook(() => useBarbersByService(1), {
        wrapper: createWrapper(),
      });

      await waitFor(() => expect(result.current.isSuccess).toBe(true));

      expect(customersApi.getBarbersByService).toHaveBeenCalledWith(1);
      expect(result.current.data).toEqual(mockBarbers);
    });

    it('should not fetch when serviceId is not provided', () => {
      const { result } = renderHook(() => useBarbersByService(0), {
        wrapper: createWrapper(),
      });

      expect(result.current.isFetching).toBe(false);
      expect(customersApi.getBarbersByService).not.toHaveBeenCalled();
    });
  });

  describe('useCreateCustomer', () => {
    it('should create customer', async () => {
      const newCustomer: CreateCustomerDto = {
        username: 'customer2',
        name: 'Jane Customer',
        contactInfo: '555-0202',
      };

      const mockResponse: CustomerDto = {
        customerId: 2,
        ...newCustomer,
      };

      vi.mocked(customersApi.create).mockResolvedValue(mockResponse);

      const { result } = renderHook(() => useCreateCustomer(), {
        wrapper: createWrapper(),
      });

      result.current.mutate(newCustomer);

      await waitFor(() => expect(result.current.isSuccess).toBe(true));

      expect(customersApi.create).toHaveBeenCalledWith(newCustomer);
      expect(result.current.data).toEqual(mockResponse);
    });
  });

  describe('useRequestAppointment', () => {
    it('should request appointment', async () => {
      const request: AppointmentRequestDto = {
        customer: {
          customerId: 1,
          username: 'customer1',
          name: 'John Customer',
          contactInfo: '555-0201',
        },
        appointment: {
          appointmentId: 0,
          status: 'Pending',
          customerId: 1,
          serviceId: 1,
          barberId: 1,
          appointmentDate: '2025-10-15T10:00:00Z',
        },
      };

      const mockResponse: AppointmentDto = {
        appointmentId: 1,
        status: 'Pending',
        customerId: 1,
        serviceId: 1,
        barberId: 1,
        appointmentDate: '2025-10-15T10:00:00Z',
      };

      vi.mocked(customersApi.requestAppointment).mockResolvedValue(mockResponse);

      const { result } = renderHook(() => useRequestAppointment(), {
        wrapper: createWrapper(),
      });

      result.current.mutate(request);

      await waitFor(() => expect(result.current.isSuccess).toBe(true));

      expect(customersApi.requestAppointment).toHaveBeenCalledWith(request);
      expect(result.current.data).toEqual(mockResponse);
    });
  });
});
