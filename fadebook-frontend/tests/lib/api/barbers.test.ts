import { describe, it, expect, vi, beforeEach } from 'vitest';
import { barbersApi } from '@/lib/api/barbers';
import { axiosInstance } from '@/lib/axios';
import type { BarberDto, CreateBarberDto } from '@/types/api';

vi.mock('@/lib/axios');

describe('barbersApi', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe('getAll', () => {
    it('should fetch all barbers', async () => {
      const mockBarbers: BarberDto[] = [
        {
          barberId: 1,
          username: 'barber1',
          name: 'John Barber',
          specialty: 'Classic Cuts',
          contactInfo: '555-0101',
        },
        {
          barberId: 2,
          username: 'barber2',
          name: 'Jane Barber',
          specialty: 'Modern Styles',
          contactInfo: '555-0102',
        },
      ];

      vi.mocked(axiosInstance.get).mockResolvedValue({ data: mockBarbers });

      const result = await barbersApi.getAll();

      expect(axiosInstance.get).toHaveBeenCalledWith('/api/barber');
      expect(result).toEqual(mockBarbers);
    });
  });

  describe('getById', () => {
    it('should fetch barber by id', async () => {
      const mockBarber: BarberDto = {
        barberId: 1,
        username: 'barber1',
        name: 'John Barber',
        specialty: 'Classic Cuts',
        contactInfo: '555-0101',
      };

      vi.mocked(axiosInstance.get).mockResolvedValue({ data: mockBarber });

      const result = await barbersApi.getById(1);

      expect(axiosInstance.get).toHaveBeenCalledWith('/api/barber/1');
      expect(result).toEqual(mockBarber);
    });
  });

  describe('create', () => {
    it('should create a new barber', async () => {
      const newBarber: CreateBarberDto = {
        username: 'barber3',
        name: 'Bob Barber',
        specialty: 'Beard Trimming',
        contactInfo: '555-0103',
      };

      const mockResponse: BarberDto = {
        barberId: 3,
        ...newBarber,
      };

      vi.mocked(axiosInstance.post).mockResolvedValue({ data: mockResponse });

      const result = await barbersApi.create(newBarber);

      expect(axiosInstance.post).toHaveBeenCalledWith('/api/barber', newBarber);
      expect(result).toEqual(mockResponse);
    });
  });

  describe('update', () => {
    it('should update an existing barber', async () => {
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

      vi.mocked(axiosInstance.put).mockResolvedValue({ data: mockResponse });

      const result = await barbersApi.update(1, updatedBarber);

      expect(axiosInstance.put).toHaveBeenCalledWith('/api/barber/1', updatedBarber);
      expect(result).toEqual(mockResponse);
    });
  });

  describe('delete', () => {
    it('should delete a barber', async () => {
      vi.mocked(axiosInstance.delete).mockResolvedValue({ data: undefined });

      await barbersApi.delete(1);

      expect(axiosInstance.delete).toHaveBeenCalledWith('/api/barber/1');
    });
  });

  describe('updateServices', () => {
    it('should update barber services', async () => {
      const serviceIds = [1, 2, 3];

      vi.mocked(axiosInstance.put).mockResolvedValue({ data: undefined });

      await barbersApi.updateServices(1, serviceIds);

      expect(axiosInstance.put).toHaveBeenCalledWith('/api/barber/1/services', serviceIds);
    });
  });
});
