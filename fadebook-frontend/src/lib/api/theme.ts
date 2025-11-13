import { axiosInstance } from '@/lib/axios';

export type ThemeValue = 'light' | 'dark' | 'system';

export const themeApi = {
  get: async (): Promise<ThemeValue> => {
    const { data } = await axiosInstance.get<{ theme: ThemeValue }>('/api/theme');
    return data.theme;
  },
  set: async (theme: ThemeValue): Promise<ThemeValue> => {
    const { data } = await axiosInstance.post<{ theme: ThemeValue }>('/api/theme', { theme });
    return data.theme;
  },
};
