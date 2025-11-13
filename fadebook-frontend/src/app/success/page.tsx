'use client';

import { useEffect, useState } from 'react';
import { useSearchParams } from 'next/navigation';
import Link from 'next/link';
import { Navigation } from '@/components/Navigation';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { CheckCircle2 } from 'lucide-react';

export default function SuccessPage() {
  const searchParams = useSearchParams();
  const [appointmentDetails, setAppointmentDetails] = useState({
    appointmentId: '',
    date: '',
    time: '',
    barberName: '',
    serviceName: '',
  });

  useEffect(() => {
    // Get appointment details from URL params or localStorage
    const appointmentId = searchParams.get('appointmentId') || localStorage.getItem('lastAppointmentId') || '';
    const date = searchParams.get('date') || localStorage.getItem('lastAppointmentDate') || '';
    const time = searchParams.get('time') || localStorage.getItem('lastAppointmentTime') || '';
    const barberName = searchParams.get('barberName') || localStorage.getItem('lastBarberName') || '';
    const serviceName = searchParams.get('serviceName') || localStorage.getItem('lastServiceName') || '';

    setAppointmentDetails({
      appointmentId,
      date,
      time,
      barberName,
      serviceName,
    });

    // Clear localStorage after displaying
    localStorage.removeItem('lastAppointmentId');
    localStorage.removeItem('lastAppointmentDate');
    localStorage.removeItem('lastAppointmentTime');
    localStorage.removeItem('lastBarberName');
    localStorage.removeItem('lastServiceName');
  }, [searchParams]);

  return (
    <div className="min-h-screen flex flex-col">
      <Navigation />
      <main className="flex-1 flex items-center justify-center py-12 px-4">
        <Card className="w-full max-w-2xl">
          <CardHeader className="text-center">
            <div className="flex justify-center mb-4">
              <CheckCircle2 className="h-16 w-16 text-green-500" />
            </div>
            <CardTitle className="text-3xl">Appointment Booked Successfully!</CardTitle>
            <CardDescription>
              Your appointment has been confirmed. We look forward to seeing you!
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-6">
            {appointmentDetails.appointmentId && (
              <div className="bg-muted/50 rounded-lg p-6 space-y-4">
                <div className="flex justify-between items-center">
                  <span className="text-sm font-medium text-muted-foreground">Appointment ID</span>
                  <Badge variant="secondary">#{appointmentDetails.appointmentId}</Badge>
                </div>

                {appointmentDetails.date && (
                  <div className="flex justify-between items-center">
                    <span className="text-sm font-medium text-muted-foreground">Date</span>
                    <span className="font-medium">{appointmentDetails.date}</span>
                  </div>
                )}

                {appointmentDetails.time && (
                  <div className="flex justify-between items-center">
                    <span className="text-sm font-medium text-muted-foreground">Time</span>
                    <span className="font-medium">{appointmentDetails.time}</span>
                  </div>
                )}

                {appointmentDetails.barberName && (
                  <div className="flex justify-between items-center">
                    <span className="text-sm font-medium text-muted-foreground">Barber</span>
                    <span className="font-medium">{appointmentDetails.barberName}</span>
                  </div>
                )}

                {appointmentDetails.serviceName && (
                  <div className="flex justify-between items-center">
                    <span className="text-sm font-medium text-muted-foreground">Service</span>
                    <span className="font-medium">{appointmentDetails.serviceName}</span>
                  </div>
                )}
              </div>
            )}

            <div className="bg-blue-50 dark:bg-blue-950 border border-blue-200 dark:border-blue-800 rounded-lg p-4">
              <h3 className="font-semibold mb-2">What's Next?</h3>
              <ul className="space-y-2 text-sm text-muted-foreground">
                <li>• You will receive a confirmation email shortly</li>
                <li>• Please arrive 5 minutes before your appointment time</li>
                <li>• If you need to cancel or reschedule, please contact us at least 24 hours in advance</li>
              </ul>
            </div>

            <div className="flex gap-4">
              <Link href="/my-appointments" className="flex-1">
                <Button variant="outline" className="w-full">
                  View My Appointments
                </Button>
              </Link>
              <Link href="/" className="flex-1">
                <Button className="w-full">
                  Back to Home
                </Button>
              </Link>
            </div>
          </CardContent>
        </Card>
      </main>
    </div>
  );
}
