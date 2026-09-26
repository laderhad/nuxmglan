import React from 'react';
import { Preset } from '../types';
import { EffectBlock } from './EffectBlock';

interface SignalChainProps {
  preset: Preset | null;
  selectedBlockId: string | null;
  onSelectBlock: (blockId: string) => void;
  onToggleBlock: (blockId: string, enabled: boolean) => void;
}

const DEFAULT_CHAIN = [
  'wahVolume', 'compression', 'efx', 'overdrive', 'amplifier', 'eq', 'noiseGate', 'modulation', 'delay', 'reverb', 'cabinet'
];

const BLOCK_COLORS: Record<string, string> = {
  amplifier: '#e94560', // red
  overdrive: '#f97316', // orange
  compression: '#eab308', // yellow
  modulation: '#3b82f6', // blue
  delay: '#22c55e', // green
  reverb: '#a855f7', // purple
  noiseGate: '#6b7280', // gray
  eq: '#14b8a6', // teal
  cabinet: '#78716c', // stone
  wahVolume: '#f472b6', // pink
  efx: '#f97316', // orange
};

const BLOCK_LABELS: Record<string, string> = {
  amplifier: 'AMP',
  overdrive: 'OD',
  compression: 'COMP',
  modulation: 'MOD',
  delay: 'DLY',
  reverb: 'RVB',
  noiseGate: 'GATE',
  eq: 'EQ',
  cabinet: 'CAB',
  wahVolume: 'WAH',
  efx: 'EFX',
};

export const SignalChain: React.FC<SignalChainProps> = ({
  preset,
  selectedBlockId,
  onSelectBlock,
  onToggleBlock
}) => {
  if (!preset) {
    return (
      <div className="h-64 flex items-center justify-center text-[#aaa] border-b border-[#1a1a2e]">
        No preset loaded
      </div>
    );
  }

  const scene = preset.sceneA; // Using Scene A for now
  const orderedBlocks = DEFAULT_CHAIN; 

  return (
    <div className="h-64 border-b border-[#1a1a2e] bg-[#0f0f0f] p-6 flex flex-col justify-center">
      <div className="text-xs font-semibold tracking-wider text-[#aaa] mb-6 uppercase">
        Signal Chain
      </div>
      
      <div className="flex items-center space-x-2 overflow-x-auto pb-4 custom-scrollbar">
        {orderedBlocks.map((blockName, index) => {
          // Type assertion to bypass complex dynamic access typing for this demo
          const blockData = (scene as any)[blockName];
          if (!blockData) return null;
          
          const isEnabled = blockName === 'cabinet' ? true : blockData.enabled;
          const isSelected = selectedBlockId === blockName;
          const modelName = blockName === 'cabinet' ? blockData.name : (blockData.modelName || blockData.effectName);
          
          return (
            <React.Fragment key={blockName}>
              <EffectBlock
                id={blockName}
                name={BLOCK_LABELS[blockName]}
                modelName={modelName}
                color={BLOCK_COLORS[blockName]}
                enabled={isEnabled}
                selected={isSelected}
                onClick={() => onSelectBlock(blockName)}
                onToggle={(enabled) => onToggleBlock(blockName, enabled)}
                hasToggle={blockName !== 'cabinet'}
              />
              
              {index < orderedBlocks.length - 1 && (
                <div className="w-6 h-0.5 bg-[#333] flex-shrink-0" />
              )}
            </React.Fragment>
          );
        })}
      </div>
    </div>
  );
};
