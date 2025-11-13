'use client';

import { useState } from 'react';
import { useBarbers, useCreateBarber, useDeleteBarber } from '@/hooks/useBarbers';
import { useServices } from '@/hooks/useCustomers';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
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
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/components/ui/dialog';
import { Checkbox } from '@/components/ui/checkbox';
import { Plus, Trash2 } from 'lucide-react';
import type { CreateBarberDto } from '@/types/api';

export function BarbersTab() {
  const { data: barbers, isLoading, error } = useBarbers();
  const { data: services } = useServices();
  const createBarber = useCreateBarber();
  const deleteBarber = useDeleteBarber();
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [formData, setFormData] = useState({
    username: '',
    name: '',
    specialty: '',
    contactInfo: '',
    serviceIds: [] as number[],
  });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    const barberData: CreateBarberDto = {
      username: formData.username,
      name: formData.name,
      specialty: formData.specialty,
      contactInfo: formData.contactInfo,
      serviceIds: formData.serviceIds,
    };

    try {
      await createBarber.mutateAsync(barberData);
      setIsDialogOpen(false);
      setFormData({
        username: '',
        name: '',
        specialty: '',
        contactInfo: '',
        serviceIds: [],
      });
    } catch (error) {
      console.error('Failed to create barber:', error);
    }
  };

  const handleDelete = async (id: number) => {
    if (confirm('Are you sure you want to delete this barber?')) {
      try {
        await deleteBarber.mutateAsync(id);
      } catch (error) {
        console.error('Failed to delete barber:', error);
      }
    }
  };

  const toggleService = (serviceId: number) => {
    setFormData(prev => ({
      ...prev,
      serviceIds: prev.serviceIds.includes(serviceId)
        ? prev.serviceIds.filter(id => id !== serviceId)
        : [...prev.serviceIds, serviceId]
    }));
  };

  if (isLoading) {
    return (
      <Card>
        <CardContent className="py-12">
          <p className="text-center text-muted-foreground">Loading barbers...</p>
        </CardContent>
      </Card>
    );
  }

  if (error) {
    return (
      <Card>
        <CardContent className="py-12">
          <p className="text-center text-destructive">Error loading barbers</p>
        </CardContent>
      </Card>
    );
  }

  return (
    <Card>
      <CardHeader>
        <div className="flex items-center justify-between">
          <div>
            <CardTitle>Barbers</CardTitle>
            <CardDescription>Manage barbers and their services</CardDescription>
          </div>
          <Dialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
            <DialogTrigger asChild>
              <Button>
                <Plus className="h-4 w-4 mr-2" />
                Add Barber
              </Button>
            </DialogTrigger>
            <DialogContent className="max-w-md">
              <DialogHeader>
                <DialogTitle>Add New Barber</DialogTitle>
                <DialogDescription>
                  Create a new barber and assign services
                </DialogDescription>
              </DialogHeader>
              <form onSubmit={handleSubmit} className="space-y-4">
                <div className="space-y-2">
                  <Label htmlFor="username">Username</Label>
                  <Input
                    id="username"
                    required
                    value={formData.username}
                    onChange={(e) =>
                      setFormData({ ...formData, username: e.target.value })
                    }
                    placeholder="barber-username"
                  />
                </div>

                <div className="space-y-2">
                  <Label htmlFor="name">Name</Label>
                  <Input
                    id="name"
                    required
                    value={formData.name}
                    onChange={(e) =>
                      setFormData({ ...formData, name: e.target.value })
                    }
                    placeholder="John Doe"
                  />
                </div>

                <div className="space-y-2">
                  <Label htmlFor="specialty">Specialty</Label>
                  <Input
                    id="specialty"
                    value={formData.specialty}
                    onChange={(e) =>
                      setFormData({ ...formData, specialty: e.target.value })
                    }
                    placeholder="Fades, Beards, etc."
                  />
                </div>

                <div className="space-y-2">
                  <Label htmlFor="contactInfo">Contact Info</Label>
                  <Input
                    id="contactInfo"
                    value={formData.contactInfo}
                    onChange={(e) =>
                      setFormData({ ...formData, contactInfo: e.target.value })
                    }
                    placeholder="Phone or email"
                  />
                </div>

                <div className="space-y-2">
                  <Label>Services</Label>
                  <div className="space-y-2 border rounded-md p-3 max-h-48 overflow-y-auto">
                    {services?.map((service) => (
                      <div key={service.serviceId} className="flex items-center space-x-2">
                        <Checkbox
                          id={`service-${service.serviceId}`}
                          checked={formData.serviceIds.includes(service.serviceId)}
                          onCheckedChange={() => toggleService(service.serviceId)}
                        />
                        <label
                          htmlFor={`service-${service.serviceId}`}
                          className="text-sm font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70 cursor-pointer"
                        >
                          {service.serviceName} - ${service.servicePrice}
                        </label>
                      </div>
                    ))}
                  </div>
                </div>

                <Button type="submit" className="w-full" disabled={createBarber.isPending}>
                  {createBarber.isPending ? 'Creating...' : 'Create Barber'}
                </Button>
              </form>
            </DialogContent>
          </Dialog>
        </div>
      </CardHeader>
      <CardContent>
        {barbers && barbers.length === 0 ? (
          <p className="text-center py-8 text-muted-foreground">No barbers found</p>
        ) : (
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>ID</TableHead>
                <TableHead>Username</TableHead>
                <TableHead>Name</TableHead>
                <TableHead>Specialty</TableHead>
                <TableHead>Contact</TableHead>
                <TableHead className="text-right">Actions</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {barbers?.map((barber) => (
                <TableRow key={barber.barberId}>
                  <TableCell>{barber.barberId}</TableCell>
                  <TableCell>
                    <Badge variant="outline">{barber.username}</Badge>
                  </TableCell>
                  <TableCell className="font-medium">{barber.name}</TableCell>
                  <TableCell>{barber.specialty || '-'}</TableCell>
                  <TableCell>{barber.contactInfo || '-'}</TableCell>
                  <TableCell className="text-right">
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={() => handleDelete(barber.barberId)}
                      disabled={deleteBarber.isPending}
                    >
                      <Trash2 className="h-4 w-4 text-destructive" />
                    </Button>
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
