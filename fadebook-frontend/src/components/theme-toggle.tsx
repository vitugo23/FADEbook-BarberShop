'use client';

import * as React from 'react';
import { Moon, Sun } from 'lucide-react';
import { useTheme } from 'next-themes';
import { themeApi, type ThemeValue } from '@/lib/api/theme';

import { Button } from '@/components/ui/button';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';

export function ThemeToggle() {
  const { setTheme } = useTheme();

  // Sync initial theme from backend cookie
  React.useEffect(() => {
    let mounted = true;
    (async () => {
      try {
        const t = await themeApi.get();
        if (mounted) setTheme(t);
      } catch {
        // ignore
      }
    })();
    return () => {
      mounted = false;
    };
  }, [setTheme]);

  const applyTheme = async (value: ThemeValue) => {
    // Optimistically update UI
    setTheme(value);
    try {
      await themeApi.set(value);
    } catch {
      // swallow; user keeps chosen theme even if persistence fails
    }
  };

  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button variant="outline" size="icon">
          <Sun className="h-[1.2rem] w-[1.2rem] rotate-0 scale-100 transition-all dark:-rotate-90 dark:scale-0" />
          <Moon className="absolute h-[1.2rem] w-[1.2rem] rotate-90 scale-0 transition-all dark:rotate-0 dark:scale-100" />
          <span className="sr-only">Toggle theme</span>
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end">
        <DropdownMenuItem onClick={() => applyTheme('light')}>
          Light
        </DropdownMenuItem>
        <DropdownMenuItem onClick={() => applyTheme('dark')}>
          Dark
        </DropdownMenuItem>
        <DropdownMenuItem onClick={() => applyTheme('system')}>
          System
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
