import React from 'react';
import { Download, Trash2, Code, Save } from 'lucide-react';
import { SavedPreset, Preset } from '../types';

interface PresetLibraryProps {
  presets: SavedPreset[];
  currentPreset: Preset | null;
  onLoadPreset: (preset: Preset) => void;
  onDeletePreset: (id: string) => void;
  onSavePreset: () => void;
  onExportJson: () => void;
  onExportPatch: () => void;
}

export const PresetLibrary: React.FC<PresetLibraryProps> = ({
  presets,
  currentPreset,
  onLoadPreset,
  onDeletePreset,
  onSavePreset,
  onExportJson,
  onExportPatch
}) => {
  return (
    <div className="flex flex-col h-full bg-[#16213e]/30">
      <div className="p-4 border-b border-[#1a1a2e] flex items-center justify-between">
        <h3 className="font-semibold text-white">Preset Library</h3>
        {currentPreset && (
          <button 
            onClick={onSavePreset}
            className="p-1.5 bg-[#e94560]/10 text-[#e94560] rounded hover:bg-[#e94560]/20 transition-colors"
            title="Save current preset"
          >
            <Save size={16} />
          </button>
        )}
      </div>

      <div className="flex-1 overflow-y-auto p-4 space-y-2">
        {presets.length === 0 ? (
          <div className="text-center text-[#555] text-sm mt-8">
            No saved presets yet
          </div>
        ) : (
          presets.map((saved) => (
            <div 
              key={saved.id} 
              className="group bg-[#1a1a2e] border border-[#16213e] rounded-lg p-3 hover:border-[#533483]/50 transition-colors cursor-pointer"
              onClick={() => onLoadPreset(saved.preset)}
            >
              <div className="flex justify-between items-start mb-1">
                <h4 className="font-medium text-sm text-[#eee] truncate pr-2">{saved.name}</h4>
                <button 
                  onClick={(e) => {
                    e.stopPropagation();
                    onDeletePreset(saved.id);
                  }}
                  className="text-[#555] hover:text-red-400 opacity-0 group-hover:opacity-100 transition-opacity"
                >
                  <Trash2 size={14} />
                </button>
              </div>
              <p className="text-xs text-[#888] truncate">{saved.originalPrompt}</p>
            </div>
          ))
        )}
      </div>

      {currentPreset && (
        <div className="p-4 border-t border-[#1a1a2e] bg-[#1a1a2e]/50 space-y-2">
          <button 
            onClick={onExportJson}
            className="w-full flex items-center justify-center gap-2 py-2 px-4 bg-[#16213e] hover:bg-[#533483]/20 border border-[#533483]/30 text-[#eee] text-sm rounded-lg transition-colors"
          >
            <Code size={16} />
            Export as JSON
          </button>
          
          <button 
            onClick={onExportPatch}
            className="w-full flex items-center justify-center gap-2 py-2 px-4 bg-[#e94560] hover:bg-[#ff4d6d] text-white text-sm font-medium rounded-lg transition-colors"
          >
            <Download size={16} />
            Export .mg30patch
          </button>
          <p className="text-[10px] text-center text-[#888] mt-2">
            Warning: Patch export is experimental
          </p>
        </div>
      )}
    </div>
  );
};
