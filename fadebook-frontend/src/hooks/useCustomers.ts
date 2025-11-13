import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { customersApi } from '@/lib/api';
import type { CreateCustomerDto, AppointmentRequestDto } from '@/types/api';

export const useCustomer = (id: number) => {
  return useQuery({
    queryKey: ['customer', id],
    queryFn: () => customersApi.getById(id),
    enabled: !!id,
  });
};

export const useServices = () => {
  return useQuery({
    queryKey: ['services'],
    queryFn: () => customersApi.getServices(),
  });
};

export const useBarbersByService = (serviceId: number) => {
  return useQuery({
    queryKey: ['barbers', 'by-service', serviceId],
    queryFn: () => customersApi.getBarbersByService(serviceId),
    enabled: !!serviceId,
  });
};

export const useCreateCustomer = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (customer: CreateCustomerDto) => customersApi.create(customer),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['customers'] });
    },
  });
};

export const useRequestAppointment = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: AppointmentRequestDto) => customersApi.requestAppointment(request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['appointments'] });
    },
  });
};
