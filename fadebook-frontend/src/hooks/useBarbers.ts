import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { barbersApi } from '@/lib/api';
import type { CreateBarberDto } from '@/types/api';

export const useBarbers = () => {
  return useQuery({
    queryKey: ['barbers'],
    queryFn: () => barbersApi.getAll(),
  });
};

export const useBarber = (id: number) => {
  return useQuery({
    queryKey: ['barber', id],
    queryFn: () => barbersApi.getById(id),
    enabled: !!id,
  });
};

export const useCreateBarber = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (barber: CreateBarberDto) => barbersApi.create(barber),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['barbers'] });
    },
  });
};

export const useUpdateBarber = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, barber }: { id: number; barber: CreateBarberDto }) =>
      barbersApi.update(id, barber),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['barber', variables.id] });
      queryClient.invalidateQueries({ queryKey: ['barbers'] });
    },
  });
};

export const useDeleteBarber = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: number) => barbersApi.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['barbers'] });
    },
  });
};

export const useUpdateBarberServices = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, serviceIds }: { id: number; serviceIds: number[] }) =>
      barbersApi.updateServices(id, serviceIds),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['barber', variables.id] });
      queryClient.invalidateQueries({ queryKey: ['barbers'] });
    },
  });
};
