import React from 'react';
import { Preset } from '../types';
import { Knob } from './Knob';

interface ParameterPanelProps {
  preset: Preset | null;
  selectedBlockId: string | null;
  onParameterChange: (blockId: string, paramName: string, value: number) => void;
}

const BLOCK_COLORS: Record<string, string> = {
  amplifier: '#e94560',
  overdrive: '#f97316',
  compression: '#eab308',
  modulation: '#3b82f6',
  delay: '#22c55e',
  reverb: '#a855f7',
  noiseGate: '#6b7280',
  eq: '#14b8a6',
  cabinet: '#78716c',
  wahVolume: '#f472b6',
  efx: '#f97316',
};

export const ParameterPanel: React.FC<ParameterPanelProps> = ({
  preset,
  selectedBlockId,
  onParameterChange
}) => {
  if (!preset || !selectedBlockId) {
    return (
      <div className="flex-1 flex items-center justify-center text-[#555] bg-[#0a0a0a]">
        Select an effect block to edit parameters
      </div>
    );
  }

  const scene = preset.sceneA;
  const blockData = (scene as any)[selectedBlockId];
  
  if (!blockData) return null;

  const color = BLOCK_COLORS[selectedBlockId] || '#555';
  
  let parameters = {};
  let modelName = '';

  if (selectedBlockId === 'amplifier') {
    parameters = {
      gain: blockData.gain,
      bass: blockData.bass,
      middle: blockData.middle,
      treble: blockData.treble,
      presence: blockData.presence,
      volume: blockData.volume
    };
    modelName = blockData.modelName;
  } else if (selectedBlockId === 'cabinet') {
    parameters = {
      micPosition: blockData.micPosition
    };
    modelName = blockData.name;
  } else {
    parameters = blockData.parameters || {};
    modelName = blockData.effectName;
  }

  return (
    <div className="flex-1 bg-[#0a0a0a] p-8 overflow-y-auto">
      <div className="max-w-3xl mx-auto">
        <div className="mb-8 border-b border-[#222] pb-4">
          <h2 className="text-2xl font-bold capitalize text-white flex items-center gap-3">
            <span className="w-3 h-3 rounded-full" style={{ backgroundColor: color }}></span>
            {selectedBlockId.replace(/([A-Z])/g, ' $1').trim()}
          </h2>
          <p className="text-[#aaa] mt-1 text-sm font-medium">Model: <span className="text-white">{modelName}</span></p>
        </div>

        <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 gap-y-12 gap-x-8">
          {Object.entries(parameters).map(([key, value]) => (
            <div key={key} className="flex flex-col items-center justify-center">
              <Knob
                value={value as number}
                onChange={(val) => onParameterChange(selectedBlockId, key, val)}
                color={color}
              />
              <span className="mt-4 text-xs font-semibold text-[#888] uppercase tracking-wider">
                {key}
              </span>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};
