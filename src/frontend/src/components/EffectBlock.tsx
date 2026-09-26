import React from 'react';
import { Power } from 'lucide-react';

interface EffectBlockProps {
  id: string;
  name: string;
  modelName: string;
  color: string;
  enabled: boolean;
  selected: boolean;
  hasToggle: boolean;
  onClick: () => void;
  onToggle: (enabled: boolean) => void;
}

export const EffectBlock: React.FC<EffectBlockProps> = ({
  name,
  modelName,
  color,
  enabled,
  selected,
  hasToggle,
  onClick,
  onToggle
}) => {
  return (
    <div 
      className={`
        flex-shrink-0 w-28 rounded-xl border-2 transition-all cursor-pointer relative overflow-hidden
        ${selected ? 'scale-105 shadow-lg z-10' : 'hover:border-[#333] z-0'}
        ${!enabled && !selected ? 'opacity-50' : 'opacity-100'}
      `}
      style={{ 
        borderColor: selected ? color : (enabled ? `${color}66` : '#222'),
        backgroundColor: enabled ? `${color}15` : '#111'
      }}
      onClick={onClick}
    >
      <div 
        className="h-1 w-full" 
        style={{ backgroundColor: enabled ? color : '#333' }} 
      />
      
      <div className="p-3">
        <div className="flex justify-between items-start mb-2">
          <span 
            className="text-xs font-bold tracking-wider"
            style={{ color: enabled ? color : '#888' }}
          >
            {name}
          </span>
          
          {hasToggle && (
            <button 
              onClick={(e) => {
                e.stopPropagation();
                onToggle(!enabled);
              }}
              className="p-1 rounded-full hover:bg-white/10 transition-colors"
            >
              <Power 
                size={12} 
                color={enabled ? color : '#555'} 
                className={enabled ? '' : 'opacity-50'} 
              />
            </button>
          )}
        </div>
        
        <div className="text-[10px] text-white/80 font-medium truncate mt-1">
          {modelName || 'Empty'}
        </div>
      </div>
    </div>
  );
};
