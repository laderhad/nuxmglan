import { ToneRequest, ToneResponse, EquipmentCatalog, SavedPreset, Preset } from '../types';

const API_BASE = '/api';

async function fetchJson<T>(url: string, options?: RequestInit): Promise<T> {
  const response = await fetch(`${API_BASE}${url}`, {
    headers: { 'Content-Type': 'application/json', ...(options?.headers || {}) },
    ...options,
  });
  if (!response.ok) {
    throw new Error(`API error: ${response.status} ${response.statusText}`);
  }
  return response.json();
}

export const api = {
  generateTone: (request: ToneRequest) =>
    fetchJson<ToneResponse>('/tone/generate', {
      method: 'POST',
      body: JSON.stringify(request),
    }),

  getCatalog: () => fetchJson<EquipmentCatalog>('/catalog'),

  getPresets: () => fetchJson<SavedPreset[]>('/presets'),

  savePreset: (preset: SavedPreset) =>
    fetchJson<SavedPreset>('/presets', {
      method: 'POST',
      body: JSON.stringify(preset),
    }),

  deletePreset: (id: string) =>
    fetch(`${API_BASE}/presets/${id}`, { method: 'DELETE' }),

  exportJson: (preset: Preset) =>
    fetchJson<Preset>('/patch/export-json', {
      method: 'POST',
      body: JSON.stringify(preset),
    }),

  exportPatch: async (preset: Preset) => {
    const response = await fetch(`${API_BASE}/patch/export`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(preset),
    });
    if (!response.ok) {
      throw new Error(`API error: ${response.status} ${response.statusText}`);
    }
    return response.blob();
  },
};
