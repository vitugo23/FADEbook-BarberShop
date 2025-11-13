'use client';

import { useState } from 'react';
import { Navigation } from '@/components/Navigation';
import { useAppointmentsByDate } from '@/hooks/useAppointments';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Badge } from '@/components/ui/badge';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table';

export default function AppointmentsPage() {
  const [selectedDate, setSelectedDate] = useState(
    new Date().toISOString().split('T')[0]
  );

  const { data: appointments, isLoading, error } = useAppointmentsByDate(selectedDate);

  return (
    <div className="min-h-screen flex flex-col">
      <Navigation />
      <main className="flex-1 py-8 px-4">
        <div className="container mx-auto">
          <h1 className="text-4xl font-bold mb-8">Appointments</h1>

          <Card className="mb-8">
            <CardHeader>
              <CardTitle>Filter by Date</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="flex gap-4 items-end">
                <div className="flex-1 max-w-sm">
                  <Label htmlFor="date">Select Date</Label>
                  <Input
                    id="date"
                    type="date"
                    value={selectedDate}
                    onChange={(e) => setSelectedDate(e.target.value)}
                  />
                </div>
              </div>
            </CardContent>
          </Card>

          {isLoading && (
            <div className="text-center py-12">
              <p className="text-muted-foreground">Loading appointments...</p>
            </div>
          )}

          {error && (
            <div className="text-center py-12">
              <p className="text-destructive">Error loading appointments. Please try again later.</p>
            </div>
          )}

          {appointments && appointments.length === 0 && (
            <div className="text-center py-12">
              <p className="text-muted-foreground">No appointments found for this date.</p>
            </div>
          )}

          {appointments && appointments.length > 0 && (
            <Card>
              <CardContent className="pt-6">
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
                    {appointments.map((appointment) => (
                      <TableRow key={appointment.appointmentId}>
                        <TableCell>{appointment.appointmentId}</TableCell>
                        <TableCell>
                          {new Date(appointment.appointmentDate).toLocaleString()}
                        </TableCell>
                        <TableCell>{appointment.customerId}</TableCell>
                        <TableCell>{appointment.barberId}</TableCell>
                        <TableCell>{appointment.serviceId}</TableCell>
                        <TableCell>
                          <Badge
                            variant={
                              appointment.status === 'Confirmed'
                                ? 'default'
                                : appointment.status === 'Pending'
                                ? 'secondary'
                                : 'outline'
                            }
                          >
                            {appointment.status}
                          </Badge>
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </CardContent>
            </Card>
          )}
        </div>
      </main>
    </div>
  );
}
