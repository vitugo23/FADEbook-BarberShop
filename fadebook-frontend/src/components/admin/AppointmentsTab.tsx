'use client';

import { useState, useEffect } from 'react';
import { axiosInstance } from '@/lib/axios';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Calendar } from 'lucide-react';
import type { AppointmentDto } from '@/types/api';

export function AppointmentsTab() {
  const [appointments, setAppointments] = useState<AppointmentDto[]>([]);
  const [filteredAppointments, setFilteredAppointments] = useState<AppointmentDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');
  const [selectedDate, setSelectedDate] = useState('');
  const [statusFilter, setStatusFilter] = useState('all');

  useEffect(() => {
    if (selectedDate) {
      fetchAppointmentsByDate(selectedDate);
    }
  }, [selectedDate]);

  useEffect(() => {
    if (statusFilter === 'all') {
      setFilteredAppointments(appointments);
    } else {
      setFilteredAppointments(
        appointments.filter((apt) => apt.status.toLowerCase() === statusFilter.toLowerCase())
      );
    }
  }, [statusFilter, appointments]);

  const fetchAppointmentsByDate = async (date: string) => {
    try {
      const { data } = await axiosInstance.get<AppointmentDto[]>(
        `/api/appointment/by-date?date=${date}`
      );
      setAppointments(data);
      setFilteredAppointments(data);
    } catch (err) {
      console.error('Failed to fetch appointments:', err);
      setError('Failed to load appointments');
    } finally {
      setIsLoading(false);
    }
  };

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

  return (
    <Card>
      <CardHeader>
        <div className="flex items-center justify-between">
          <div>
            <CardTitle>Appointments</CardTitle>
            <CardDescription>View and filter appointments</CardDescription>
          </div>
        </div>
      </CardHeader>
      <CardContent className="space-y-6">
        <div className="flex gap-4">
          <div className="flex-1">
            <Label htmlFor="date">Filter by Date</Label>
            <div className="relative">
              <Calendar className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
              <Input
                id="date"
                type="date"
                value={selectedDate}
                onChange={(e) => setSelectedDate(e.target.value)}
                className="pl-8"
              />
            </div>
          </div>
          <div className="w-48">
            <Label htmlFor="status">Filter by Status</Label>
            <Select value={statusFilter} onValueChange={setStatusFilter}>
              <SelectTrigger id="status">
                <SelectValue placeholder="All statuses" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">All Statuses</SelectItem>
                <SelectItem value="pending">Pending</SelectItem>
                <SelectItem value="confirmed">Confirmed</SelectItem>
                <SelectItem value="completed">Completed</SelectItem>
                <SelectItem value="cancelled">Cancelled</SelectItem>
              </SelectContent>
            </Select>
          </div>
        </div>

        {!selectedDate ? (
          <div className="text-center py-12">
            <Calendar className="h-12 w-12 mx-auto text-muted-foreground mb-4" />
            <p className="text-muted-foreground">Select a date to view appointments</p>
          </div>
        ) : isLoading ? (
          <p className="text-center py-8 text-muted-foreground">Loading appointments...</p>
        ) : error ? (
          <p className="text-center py-8 text-destructive">{error}</p>
        ) : filteredAppointments.length === 0 ? (
          <p className="text-center py-8 text-muted-foreground">
            No appointments found for this date
            {statusFilter !== 'all' && ` with status "${statusFilter}"`}
          </p>
        ) : (
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>ID</TableHead>
                <TableHead>Date & Time</TableHead>
                <TableHead>Customer ID</TableHead>
                <TableHead>Barber ID</TableHead>
                <TableHead>Service ID</TableHead>
                <TableHead>Status</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {filteredAppointments.map((appointment) => (
                <TableRow key={appointment.appointmentId}>
                  <TableCell>{appointment.appointmentId}</TableCell>
                  <TableCell>
                    {new Date(appointment.appointmentDate).toLocaleString()}
                  </TableCell>
                  <TableCell>
                    <Badge variant="outline">{appointment.customerId}</Badge>
                  </TableCell>
                  <TableCell>
                    <Badge variant="outline">{appointment.barberId}</Badge>
                  </TableCell>
                  <TableCell>
                    <Badge variant="outline">{appointment.serviceId}</Badge>
                  </TableCell>
                  <TableCell>
                    <Badge variant={getStatusVariant(appointment.status)}>
                      {appointment.status}
                    </Badge>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        )}
      </CardContent>
    </Card>
  );
}
