import { describe, it, expect, vi, beforeEach } from 'vitest';
import { customersApi } from '@/lib/api/customers';
import { axiosInstance } from '@/lib/axios';
import type { CustomerDto, CreateCustomerDto, ServiceDto, BarberDto, AppointmentDto, AppointmentRequestDto } from '@/types/api';

vi.mock('@/lib/axios');

describe('customersApi', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe('create', () => {
    it('should create a new customer', async () => {
      const newCustomer: CreateCustomerDto = {
        username: 'customer1',
        name: 'John Customer',
        contactInfo: '555-0201',
      };

      const mockResponse: CustomerDto = {
        customerId: 1,
        ...newCustomer,
      };

      vi.mocked(axiosInstance.post).mockResolvedValue({ data: mockResponse });

      const result = await customersApi.create(newCustomer);

      expect(axiosInstance.post).toHaveBeenCalledWith('/api/customerappointment', newCustomer);
      expect(result).toEqual(mockResponse);
    });
  });

  describe('getById', () => {
    it('should fetch customer by id', async () => {
      const mockCustomer: CustomerDto = {
        customerId: 1,
        username: 'customer1',
        name: 'John Customer',
        contactInfo: '555-0201',
      };

      vi.mocked(axiosInstance.get).mockResolvedValue({ data: mockCustomer });

      const result = await customersApi.getById(1);

      expect(axiosInstance.get).toHaveBeenCalledWith('/customer/1');
      expect(result).toEqual(mockCustomer);
    });
  });

  describe('getServices', () => {
    it('should fetch all available services', async () => {
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

      vi.mocked(axiosInstance.get).mockResolvedValue({ data: mockServices });

      const result = await customersApi.getServices();

      expect(axiosInstance.get).toHaveBeenCalledWith('/api/customer/services');
      expect(result).toEqual(mockServices);
    });
  });

  describe('getBarbersByService', () => {
    it('should fetch barbers by service id', async () => {
      const mockBarbers: BarberDto[] = [
        {
          barberId: 1,
          username: 'barber1',
          name: 'John Barber',
          specialty: 'Classic Cuts',
          contactInfo: '555-0101',
        },
      ];

      vi.mocked(axiosInstance.get).mockResolvedValue({ data: mockBarbers });

      const result = await customersApi.getBarbersByService(1);

      expect(axiosInstance.get).toHaveBeenCalledWith('/api/customer/barbers-by-service/1');
      expect(result).toEqual(mockBarbers);
    });
  });

  describe('requestAppointment', () => {
    it('should request a new appointment', async () => {
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

      vi.mocked(axiosInstance.post).mockResolvedValue({ data: mockResponse });

      const result = await customersApi.requestAppointment(request);

      expect(axiosInstance.post).toHaveBeenCalledWith('/api/customer/request-appointment', request);
      expect(result).toEqual(mockResponse);
    });
  });
});
