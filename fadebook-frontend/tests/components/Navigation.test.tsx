import { describe, it, expect, vi } from 'vitest';
import { render, screen } from '@testing-library/react';
import { Navigation } from '@/components/Navigation';

// Mock next/navigation
vi.mock('next/navigation', () => ({
  usePathname: vi.fn(() => '/'),
  useRouter: vi.fn(() => ({
    push: vi.fn(),
    replace: vi.fn(),
    refresh: vi.fn(),
  })),
}));

// Mock next/link
vi.mock('next/link', () => ({
  default: ({ children, href, className }: { children: React.ReactNode; href: string; className?: string }) => (
    <a href={href} className={className}>
      {children}
    </a>
  ),
}));

describe('Navigation', () => {
  it('should render the brand name', () => {
    render(<Navigation />);
    expect(screen.getByText('Fadebook')).toBeInTheDocument();
  });

  it('should render all navigation links', () => {
    render(<Navigation />);

    expect(screen.getByText('Home')).toBeInTheDocument();
    expect(screen.getByText('My Appointments')).toBeInTheDocument();
    expect(screen.getByText('Barbers')).toBeInTheDocument();
    expect(screen.getByText('Book Appointment')).toBeInTheDocument();
    expect(screen.getByText('Admin')).toBeInTheDocument();
  });

  it('should have correct hrefs for all links', () => {
    render(<Navigation />);

    const homeLink = screen.getByText('Home').closest('a');
    const appointmentsLink = screen.getByText('My Appointments').closest('a');
    const barbersLink = screen.getByText('Barbers').closest('a');
    const bookLink = screen.getByText('Book Appointment').closest('a');
    const adminLink = screen.getByText('Admin').closest('a');

    expect(homeLink).toHaveAttribute('href', '/');
    expect(appointmentsLink).toHaveAttribute('href', '/my-appointments');
    expect(barbersLink).toHaveAttribute('href', '/barbers');
    expect(bookLink).toHaveAttribute('href', '/book');
    expect(adminLink).toHaveAttribute('href', '/admin');
  });

  it('should apply active styles to current path', async () => {
    const nextNav = await import('next/navigation');
    vi.mocked(nextNav.usePathname).mockReturnValue('/my-appointments');

    render(<Navigation />);

    const appointmentsLink = screen.getByText('My Appointments').closest('a');
    expect(appointmentsLink?.className).toContain('bg-primary');
    expect(appointmentsLink?.className).toContain('text-primary-foreground');
  });

  it('should apply hover styles to non-active links', async () => {
    const nextNav = await import('next/navigation');
    vi.mocked(nextNav.usePathname).mockReturnValue('/');

    render(<Navigation />);

    const appointmentsLink = screen.getByText('My Appointments').closest('a');
    expect(appointmentsLink?.className).toContain('text-muted-foreground');
    expect(appointmentsLink?.className).toContain('hover:bg-accent');
  });
});
