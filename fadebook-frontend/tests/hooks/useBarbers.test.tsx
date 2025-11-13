import { describe, it, expect, vi, beforeEach } from 'vitest';
import { renderHook, waitFor } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { useBarbers, useBarber, useCreateBarber, useUpdateBarber, useDeleteBarber, useUpdateBarberServices } from '@/hooks/useBarbers';
import { barbersApi } from '@/lib/api';
import type { BarberDto, CreateBarberDto } from '@/types/api';

vi.mock('@/lib/api', () => ({
  barbersApi: {
    getAll: vi.fn(),
    getById: vi.fn(),
    create: vi.fn(),
    update: vi.fn(),
    delete: vi.fn(),
    updateServices: vi.fn(),
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

describe('useBarbers hooks', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe('useBarbers', () => {
    it('should fetch all barbers', async () => {
      const mockBarbers: BarberDto[] = [
        {
          barberId: 1,
          username: 'barber1',
          name: 'John Barber',
          specialty: 'Classic Cuts',
          contactInfo: '555-0101',
        },
      ];

      vi.mocked(barbersApi.getAll).mockResolvedValue(mockBarbers);

      const { result } = renderHook(() => useBarbers(), {
        wrapper: createWrapper(),
      });

      await waitFor(() => expect(result.current.isSuccess).toBe(true));

      expect(barbersApi.getAll).toHaveBeenCalled();
      expect(result.current.data).toEqual(mockBarbers);
    });
  });

  describe('useBarber', () => {
    it('should fetch barber by id', async () => {
      const mockBarber: BarberDto = {
        barberId: 1,
        username: 'barber1',
        name: 'John Barber',
        specialty: 'Classic Cuts',
        contactInfo: '555-0101',
      };

      vi.mocked(barbersApi.getById).mockResolvedValue(mockBarber);

      const { result } = renderHook(() => useBarber(1), {
        wrapper: createWrapper(),
      });

      await waitFor(() => expect(result.current.isSuccess).toBe(true));

      expect(barbersApi.getById).toHaveBeenCalledWith(1);
      expect(result.current.data).toEqual(mockBarber);
    });

    it('should not fetch when id is not provided', () => {
      const { result } = renderHook(() => useBarber(0), {
        wrapper: createWrapper(),
      });

      expect(result.current.isFetching).toBe(false);
      expect(barbersApi.getById).not.toHaveBeenCalled();
    });
  });

  describe('useCreateBarber', () => {
    it('should create barber', async () => {
      const newBarber: CreateBarberDto = {
        username: 'barber2',
        name: 'Jane Barber',
        specialty: 'Modern Styles',
        contactInfo: '555-0102',
      };

      const mockResponse: BarberDto = {
        barberId: 2,
        ...newBarber,
      };

      vi.mocked(barbersApi.create).mockResolvedValue(mockResponse);

      const { result } = renderHook(() => useCreateBarber(), {
        wrapper: createWrapper(),
      });

      result.current.mutate(newBarber);

      await waitFor(() => expect(result.current.isSuccess).toBe(true));

      expect(barbersApi.create).toHaveBeenCalledWith(newBarber);
      expect(result.current.data).toEqual(mockResponse);
    });
  });

  describe('useUpdateBarber', () => {
    it('should update barber', async () => {
      const updatedBarber: CreateBarberDto = {
        username: 'barber1',
        name: 'John Updated',
        specialty: 'All Styles',
        contactInfo: '555-0199',
      };

      const mockResponse: BarberDto = {
        barberId: 1,
        ...updatedBarber,
      };

      vi.mocked(barbersApi.update).mockResolvedValue(mockResponse);

      const { result } = renderHook(() => useUpdateBarber(), {
        wrapper: createWrapper(),
      });

      result.current.mutate({ id: 1, barber: updatedBarber });

      await waitFor(() => expect(result.current.isSuccess).toBe(true));

      expect(barbersApi.update).toHaveBeenCalledWith(1, updatedBarber);
      expect(result.current.data).toEqual(mockResponse);
    });
  });

  describe('useDeleteBarber', () => {
    it('should delete barber', async () => {
      vi.mocked(barbersApi.delete).mockResolvedValue(undefined);

      const { result } = renderHook(() => useDeleteBarber(), {
        wrapper: createWrapper(),
      });

      result.current.mutate(1);

      await waitFor(() => expect(result.current.isSuccess).toBe(true));

      expect(barbersApi.delete).toHaveBeenCalledWith(1);
    });
  });

  describe('useUpdateBarberServices', () => {
    it('should update barber services', async () => {
      const serviceIds = [1, 2, 3];

      vi.mocked(barbersApi.updateServices).mockResolvedValue(undefined);

      const { result } = renderHook(() => useUpdateBarberServices(), {
        wrapper: createWrapper(),
      });

      result.current.mutate({ id: 1, serviceIds });

      await waitFor(() => expect(result.current.isSuccess).toBe(true));

      expect(barbersApi.updateServices).toHaveBeenCalledWith(1, serviceIds);
    });
  });
});
