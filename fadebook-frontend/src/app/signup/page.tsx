'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import Link from 'next/link';
import { Navigation } from '@/components/Navigation';
import { axiosInstance } from '@/lib/axios';
import { customersApi } from '@/lib/api/customers';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import type { CustomerDto } from '@/types/api';

export default function SignUpPage() {
  const router = useRouter();
  const [formData, setFormData] = useState({
    username: '',
    name: '',
    contactInfo: '',
  });
  const [error, setError] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [isCheckingUsername, setIsCheckingUsername] = useState(false);
  const [isUsernameTaken, setIsUsernameTaken] = useState<boolean | null>(null);
  const [usernameCheckError, setUsernameCheckError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setIsLoading(true);

    const customerData: CustomerDto = {
      customerId: 0, // Will be set by backend
      username: formData.username,
      name: formData.name,
      contactInfo: formData.contactInfo,
    };

    try {
      console.log('Attempting signup with data:', customerData);
      
      // Call signup API
      const { data } = await axiosInstance.post<CustomerDto>(
        '/api/customeraccount/signup',
        customerData
      );
      
      console.log('Signup successful:', data);
      
      // Store user info in localStorage
      localStorage.setItem('username', data.username);
      localStorage.setItem('customerId', data.customerId.toString());
      localStorage.setItem('isAuthenticated', 'true');
      
      // Redirect to booking page
      router.push('/book');
    } catch (err: any) {
      console.error('Signup error:', err);
      console.error('Error response:', err.response);
      console.error('Error response data:', err.response?.data);
      console.error('Error status:', err.response?.status);
      
      if (err.response?.status === 409) {
        setError('Username already taken. Please choose a different username.');
      } else if (err.response?.status === 400) {
        const errorMsg = err.response?.data?.message || err.response?.data?.title || 'Invalid data. Please check all fields.';
        setError(errorMsg);
      } else if (err.code === 'ERR_NETWORK') {
        setError('Cannot connect to server. Please make sure the API is running on http://localhost:5288');
      } else {
        setError(err.response?.data?.message || 'Failed to create account. Please try again.');
      }
    } finally {
      setIsLoading(false);
    }
  };

  // Debounced username availability check
  
  // use inline debounce to avoid adding a new hook file
  const [debounceTimer, setDebounceTimer] = useState<NodeJS.Timeout | null>(null);
  const onUsernameChange = (value: string) => {
    setFormData({ ...formData, username: value });
    setError('');
    setUsernameCheckError('');
    setIsUsernameTaken(null);
    if (debounceTimer) clearTimeout(debounceTimer);
    const t = setTimeout(async () => {
      const uname = value.trim();
      if (!uname) {
        setIsCheckingUsername(false);
        setIsUsernameTaken(null);
        return;
      }
      setIsCheckingUsername(true);
      try {
        const exists = await customersApi.usernameExists(uname);
        setIsUsernameTaken(exists);
      } catch (e: any) {
        // Backend returns 400 for blank; we already guard. Show generic message otherwise.
        setUsernameCheckError(e?.response?.data?.message || 'Unable to verify username.');
        setIsUsernameTaken(null);
      } finally {
        setIsCheckingUsername(false);
      }
    }, 400);
    setDebounceTimer(t);
  };

  return (
    <div className="min-h-screen flex flex-col">
      <Navigation />
      <main className="flex-1 flex items-center justify-center py-12 px-4">
        <Card className="w-full max-w-md">
          <CardHeader>
            <CardTitle>Sign Up</CardTitle>
            <CardDescription>Create an account to book appointments</CardDescription>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit} className="space-y-4">
              {error && (
                <div className="p-3 text-sm text-destructive bg-destructive/10 rounded-md">
                  {error}
                </div>
              )}

              <div className="space-y-2">
                <Label htmlFor="username">Username</Label>
                <Input
                  id="username"
                  type="text"
                  required
                  value={formData.username}
                  onChange={(e) => onUsernameChange(e.target.value)}
                  placeholder="Choose a username"
                />
                <div className="text-sm min-h-5">
                  {isCheckingUsername && (
                    <span className="text-muted-foreground">Checking username...</span>
                  )}
                  {!isCheckingUsername && usernameCheckError && (
                    <span className="text-destructive">{usernameCheckError}</span>
                  )}
                  {!isCheckingUsername && isUsernameTaken === true && (
                    <span className="text-destructive">Username already taken.</span>
                  )}
                  {!isCheckingUsername && isUsernameTaken === false && (
                    <span className="text-primary">Username available.</span>
                  )}
                </div>
              </div>

              <div className="space-y-2">
                <Label htmlFor="name">Full Name</Label>
                <Input
                  id="name"
                  type="text"
                  required
                  value={formData.name}
                  onChange={(e) =>
                    setFormData({ ...formData, name: e.target.value })
                  }
                  placeholder="Enter your full name"
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="contactInfo">Contact Info</Label>
                <Input
                  id="contactInfo"
                  type="text"
                  required
                  value={formData.contactInfo}
                  onChange={(e) =>
                    setFormData({ ...formData, contactInfo: e.target.value })
                  }
                  placeholder="Email or phone number"
                />
              </div>

              <Button
                type="submit"
                className="w-full"
                disabled={isLoading || isUsernameTaken === true}
              >
                {isLoading ? 'Creating account...' : 'Sign Up'}
              </Button>

              <div className="text-center text-sm">
                <span className="text-muted-foreground">Already have an account? </span>
                <Link href="/signin" className="text-primary hover:underline">
                  Sign in
                </Link>
              </div>
            </form>
          </CardContent>
        </Card>
      </main>
    </div>
  );
}
