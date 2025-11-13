'use client';

import { Navigation } from '@/components/Navigation';
import { useBarbers } from '@/hooks/useBarbers';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';

export default function BarbersPage() {
  const { data: barbers, isLoading, error } = useBarbers();

  return (
    <div className="min-h-screen flex flex-col">
      <Navigation />
      <main className="flex-1 py-8 px-4">
        <div className="container mx-auto">
          <h1 className="text-4xl font-bold mb-8">Our Barbers</h1>

          {isLoading && (
            <div className="text-center py-12">
              <p className="text-muted-foreground">Loading barbers...</p>
            </div>
          )}

          {error && (
            <div className="text-center py-12">
              <p className="text-destructive">Error loading barbers. Please try again later.</p>
            </div>
          )}

          {barbers && barbers.length === 0 && (
            <div className="text-center py-12">
              <p className="text-muted-foreground">No barbers found.</p>
            </div>
          )}

          <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-6">
            {barbers?.map((barber) => (
              <Card key={barber.barberId}>
                <CardHeader>
                  <div className="flex items-start justify-between">
                    <div>
                      <CardTitle>{barber.name}</CardTitle>
                      <CardDescription>@{barber.username}</CardDescription>
                    </div>
                    {barber.specialty && (
                      <Badge variant="secondary">{barber.specialty}</Badge>
                    )}
                  </div>
                </CardHeader>
                <CardContent>
                  {barber.contactInfo && (
                    <p className="text-sm text-muted-foreground">
                      Contact: {barber.contactInfo}
                    </p>
                  )}
                </CardContent>
              </Card>
            ))}
          </div>
        </div>
      </main>
    </div>
  );
}
