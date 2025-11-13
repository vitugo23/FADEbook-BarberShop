'use client';

import { useState } from 'react';
import { useServices, useCreateService, useDeleteService } from '@/hooks/useServices';
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
import { Plus, Trash2 } from 'lucide-react';
import type { CreateServiceDto } from '@/hooks/useServices';

export function ServicesTab() {
  const { data: services, isLoading, error } = useServices();
  const createService = useCreateService();
  const deleteService = useDeleteService();
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [formData, setFormData] = useState({
    serviceName: '',
    servicePrice: '',
  });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    const serviceData: CreateServiceDto = {
      serviceName: formData.serviceName,
      servicePrice: parseFloat(formData.servicePrice),
    };

    try {
      await createService.mutateAsync(serviceData);
      setIsDialogOpen(false);
      setFormData({
        serviceName: '',
        servicePrice: '',
      });
    } catch (error) {
      console.error('Failed to create service:', error);
    }
  };

  const handleDelete = async (id: number) => {
    if (confirm('Are you sure you want to delete this service? This may affect existing appointments.')) {
      try {
        await deleteService.mutateAsync(id);
      } catch (error) {
        console.error('Failed to delete service:', error);
      }
    }
  };

  if (isLoading) {
    return (
      <Card>
        <CardContent className="py-12">
          <p className="text-center text-muted-foreground">Loading services...</p>
        </CardContent>
      </Card>
    );
  }

  if (error) {
    return (
      <Card>
        <CardContent className="py-12">
          <p className="text-center text-destructive">Error loading services</p>
        </CardContent>
      </Card>
    );
  }

  return (
    <Card>
      <CardHeader>
        <div className="flex items-center justify-between">
          <div>
            <CardTitle>Services</CardTitle>
            <CardDescription>Manage available services and pricing</CardDescription>
          </div>
          <Dialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
            <DialogTrigger asChild>
              <Button>
                <Plus className="h-4 w-4 mr-2" />
                Add Service
              </Button>
            </DialogTrigger>
            <DialogContent className="max-w-md">
              <DialogHeader>
                <DialogTitle>Add New Service</DialogTitle>
                <DialogDescription>
                  Create a new service with pricing
                </DialogDescription>
              </DialogHeader>
              <form onSubmit={handleSubmit} className="space-y-4">
                <div className="space-y-2">
                  <Label htmlFor="serviceName">Service Name</Label>
                  <Input
                    id="serviceName"
                    required
                    value={formData.serviceName}
                    onChange={(e) =>
                      setFormData({ ...formData, serviceName: e.target.value })
                    }
                    placeholder="e.g., Haircut, Beard Trim"
                  />
                </div>

                <div className="space-y-2">
                  <Label htmlFor="servicePrice">Price ($)</Label>
                  <Input
                    id="servicePrice"
                    type="number"
                    step="0.01"
                    min="0"
                    required
                    value={formData.servicePrice}
                    onChange={(e) =>
                      setFormData({ ...formData, servicePrice: e.target.value })
                    }
                    placeholder="25.00"
                  />
                </div>

                <Button type="submit" className="w-full" disabled={createService.isPending}>
                  {createService.isPending ? 'Creating...' : 'Create Service'}
                </Button>
              </form>
            </DialogContent>
          </Dialog>
        </div>
      </CardHeader>
      <CardContent>
        {services && services.length === 0 ? (
          <p className="text-center py-8 text-muted-foreground">No services found</p>
        ) : (
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>ID</TableHead>
                <TableHead>Service Name</TableHead>
                <TableHead>Price</TableHead>
                <TableHead className="text-right">Actions</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {services?.map((service) => (
                <TableRow key={service.serviceId}>
                  <TableCell>
                    {service.serviceId}
                  </TableCell>
                  <TableCell className="font-medium">
                    <Badge variant="outline">{service.serviceName}</Badge>
                  </TableCell>
                  <TableCell className="font-semibold text-green-600">
                    ${service.servicePrice.toFixed(2)}
                  </TableCell>
                  <TableCell className="text-right">
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={() => handleDelete(service.serviceId)}
                      disabled={deleteService.isPending}
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
