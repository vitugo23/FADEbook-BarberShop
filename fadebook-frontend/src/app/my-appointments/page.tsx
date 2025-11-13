'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { Navigation } from '@/components/Navigation';
import { axiosInstance } from '@/lib/axios';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table';
import { Calendar, Clock, User, Scissors } from 'lucide-react';
import type { AppointmentDto, BarberDto, ServiceDto } from '@/types/api';

interface AppointmentWithDetails extends AppointmentDto {
  barberName?: string;
  serviceName?: string;
  servicePrice?: number;
}

export default function MyAppointmentsPage() {
  const router = useRouter();
  const [appointments, setAppointments] = useState<AppointmentWithDetails[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const fetchAppointments = async () => {
      try {
        const username = localStorage.getItem('username');
        const isAuthenticated = localStorage.getItem('isAuthenticated');

        if (!isAuthenticated || !username) {
          router.push('/signin');
          return;
        }

        // Fetch appointments for this customer by username
        const { data: appointmentsData } = await axiosInstance.get<AppointmentDto[]>(
          `/api/appointment/by-username/${username}`
        );

        // Fetch barbers and services to get names
        const { data: barbers } = await axiosInstance.get<BarberDto[]>('/api/barber');
        const { data: services } = await axiosInstance.get<ServiceDto[]>('/api/customer/services');

        // Combine data
        const appointmentsWithDetails = appointmentsData.map((apt) => {
          const barber = barbers.find((b) => b.barberId === apt.barberId);
          const service = services.find((s) => s.serviceId === apt.serviceId);
          return {
            ...apt,
            barberName: barber?.name,
            serviceName: service?.serviceName,
            servicePrice: service?.servicePrice,
          };
        });

        setAppointments(appointmentsWithDetails);
      } catch (err) {
        console.error('Failed to fetch appointments:', err);
        setError('Failed to load appointments');
      } finally {
        setIsLoading(false);
      }
    };

    fetchAppointments();
  }, [router]);

  const getStatusVariant = (status: string) => {
    switch (status.toLowerCase()) {
      case 'confirmed':
        return 'default';
      case 'pending':
        return 'secondary';
      case 'cancelled':
        return 'destructive';
      case 'completed':
        return 'outline';
      default:
        return 'secondary';
    }
  };

  if (isLoading) {
    return (
      <div className="min-h-screen flex flex-col">
        <Navigation />
        <main className="flex-1 py-8 px-4">
          <div className="container mx-auto">
            <p className="text-center text-muted-foreground">Loading your appointments...</p>
          </div>
        </main>
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen flex flex-col">
        <Navigation />
        <main className="flex-1 py-8 px-4">
          <div className="container mx-auto">
            <p className="text-center text-destructive">{error}</p>
          </div>
        </main>
      </div>
    );
  }

  return (
    <div className="min-h-screen flex flex-col">
      <Navigation />
      <main className="flex-1 py-8 px-4">
        <div className="container mx-auto">
          <h1 className="text-4xl font-bold mb-8">My Appointments</h1>

          {appointments.length === 0 ? (
            <Card>
              <CardContent className="py-12">
                <div className="text-center">
                  <Calendar className="h-12 w-12 mx-auto text-muted-foreground mb-4" />
                  <p className="text-muted-foreground">You don't have any appointments yet.</p>
                  <p className="text-sm text-muted-foreground mt-2">
                    Book your first appointment to get started!
                  </p>
                </div>
              </CardContent>
            </Card>
          ) : (
            <div className="grid gap-6">
              {appointments.map((appointment) => (
                <Card key={appointment.appointmentId}>
                  <CardHeader>
                    <div className="flex items-center justify-between">
                      <CardTitle className="text-xl">
                        Appointment #{appointment.appointmentId}
                      </CardTitle>
                      <Badge variant={getStatusVariant(appointment.status)}>
                        {appointment.status}
                      </Badge>
                    </div>
                    <CardDescription>
                      {new Date(appointment.appointmentDate).toLocaleDateString('en-US', {
                        weekday: 'long',
                        year: 'numeric',
                        month: 'long',
                        day: 'numeric',
                      })}
                    </CardDescription>
                  </CardHeader>
                  <CardContent>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                      <div className="flex items-center gap-3">
                        <Clock className="h-5 w-5 text-muted-foreground" />
                        <div>
                          <p className="text-sm font-medium">Time</p>
                          <p className="text-sm text-muted-foreground">
                            {new Date(appointment.appointmentDate).toLocaleTimeString('en-US', {
                              hour: '2-digit',
                              minute: '2-digit',
                            })}
                          </p>
                        </div>
                      </div>

                      <div className="flex items-center gap-3">
                        <User className="h-5 w-5 text-muted-foreground" />
                        <div>
                          <p className="text-sm font-medium">Barber</p>
                          <p className="text-sm text-muted-foreground">
                            {appointment.barberName || `Barber #${appointment.barberId}`}
                          </p>
                        </div>
                      </div>

                      <div className="flex items-center gap-3">
                        <Scissors className="h-5 w-5 text-muted-foreground" />
                        <div>
                          <p className="text-sm font-medium">Service</p>
                          <p className="text-sm text-muted-foreground">
                            {appointment.serviceName || `Service #${appointment.serviceId}`}
                          </p>
                        </div>
                      </div>

                      {appointment.servicePrice && (
                        <div className="flex items-center gap-3">
                          <div className="h-5 w-5 flex items-center justify-center text-muted-foreground font-bold">
                            $
                          </div>
                          <div>
                            <p className="text-sm font-medium">Price</p>
                            <p className="text-sm text-muted-foreground">
                              ${appointment.servicePrice.toFixed(2)}
                            </p>
                          </div>
                        </div>
                      )}
                    </div>
                  </CardContent>
                </Card>
              ))}
            </div>
          )}
        </div>
      </main>
    </div>
  );
}
