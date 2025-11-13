'use client';

import { useState } from 'react';
import { Navigation } from '@/components/Navigation';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { BarbersTab } from '@/components/admin/BarbersTab';
import { CustomersTab } from '@/components/admin/CustomersTab';
import { AppointmentsTab } from '@/components/admin/AppointmentsTab';
import { ServicesTab } from '@/components/admin/ServicesTab';

export default function AdminPage() {
  return (
    <div className="min-h-screen flex flex-col">
      <Navigation />
      <main className="flex-1 py-8 px-4">
        <div className="container mx-auto">
          <div className="mb-8">
            <h1 className="text-4xl font-bold mb-2">Admin Dashboard</h1>
            <p className="text-muted-foreground">
              Manage barbers, services, customers, and appointments
            </p>
          </div>

          <Tabs defaultValue="barbers" className="space-y-6">
            <TabsList className="grid w-full grid-cols-4 max-w-2xl">
              <TabsTrigger value="barbers">Barbers</TabsTrigger>
              <TabsTrigger value="services">Services</TabsTrigger>
              <TabsTrigger value="customers">Customers</TabsTrigger>
              <TabsTrigger value="appointments">Appointments</TabsTrigger>
            </TabsList>

            <TabsContent value="barbers">
              <BarbersTab />
            </TabsContent>

            <TabsContent value="services">
              <ServicesTab />
            </TabsContent>

            <TabsContent value="customers">
              <CustomersTab />
            </TabsContent>

            <TabsContent value="appointments">
              <AppointmentsTab />
            </TabsContent>
          </Tabs>
        </div>
      </main>
    </div>
  );
}
