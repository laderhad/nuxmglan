export interface Preset {
  index: number;
  name: string;
  sceneA: Scene;
  sceneB: Scene;
  sceneC: Scene;
  cabinet: CabinetIR;
}

export interface Scene {
  amplifier: AmplifierSettings;
  compression: EffectSettings;
  overdrive: EffectSettings;
  modulation: EffectSettings;
  delay: EffectSettings;
  reverb: EffectSettings;
  noiseGate: EffectSettings;
  eq: EffectSettings;
  efx: EffectSettings;
  wahVolume: EffectSettings;
  signalChain: number[];
  presetName: string;
  masterLevel: number;
  tempo: number;
}

export interface AmplifierSettings {
  modelId: string;
  modelName: string;
  enabled: boolean;
  gain: number;
  bass: number;
  middle: number;
  treble: number;
  volume: number;
  presence: number;
}

export interface EffectSettings {
  effectId: string;
  effectName: string;
  enabled: boolean;
  parameters: Record<string, number>;
}

export interface CabinetIR {
  name: string;
  isUserIR: boolean;
  micType: string;
  micPosition: number;
}

export interface ToneRequest {
  prompt: string;
}

export interface ToneResponse {
  preset: Preset;
  explanation: string;
  warnings: string[];
}

export interface EquipmentItem {
  id: string;
  name: string;
  basedOn: string;
  category: string;
  parameters: ParameterDefinition[];
}

export interface ParameterDefinition {
  name: string;
  minValue: number;
  maxValue: number;
  defaultValue: number;
}

export interface EquipmentCatalog {
  amplifiers: EquipmentItem[];
  cabinets: EquipmentItem[];
  overdrives: EquipmentItem[];
  modulations: EquipmentItem[];
  delays: EquipmentItem[];
  reverbs: EquipmentItem[];
  compressions: EquipmentItem[];
  eqs: EquipmentItem[];
  noiseGates: EquipmentItem[];
  wahVolumes: EquipmentItem[];
  efxs: EquipmentItem[];
}

export interface SavedPreset {
  id: string;
  name: string;
  originalPrompt: string;
  preset: Preset;
  createdAt: string;
  updatedAt: string;
}

export interface ChatMessage {
  id: string;
  role: 'user' | 'assistant';
  content: string;
  preset?: Preset;
  timestamp: Date;
}
