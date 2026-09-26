import React, { useState, useEffect, useRef } from 'react';

interface KnobProps {
  value: number;
  onChange: (value: number) => void;
  color: string;
  min?: number;
  max?: number;
}

export const Knob: React.FC<KnobProps> = ({ 
  value, 
  onChange, 
  color,
  min = 0,
  max = 100
}) => {
  const [isDragging, setIsDragging] = useState(false);
  const knobRef = useRef<HTMLDivElement>(null);

  // Convert value to percentage for rotation (270 degree range: -135 to +135)
  const percentage = (value - min) / (max - min);
  const rotation = -135 + (percentage * 270);

  useEffect(() => {
    const handleMouseMove = (e: MouseEvent) => {
      if (!isDragging) return;
      
      // Simple vertical drag mapping
      const deltaY = -e.movementY;
      let newVal = value + (deltaY * ((max - min) / 100)); // Adjust sensitivity
      
      // Clamp
      newVal = Math.max(min, Math.min(max, newVal));
      
      onChange(Math.round(newVal));
    };

    const handleMouseUp = () => {
      setIsDragging(false);
    };

    if (isDragging) {
      window.addEventListener('mousemove', handleMouseMove);
      window.addEventListener('mouseup', handleMouseUp);
    }

    return () => {
      window.removeEventListener('mousemove', handleMouseMove);
      window.removeEventListener('mouseup', handleMouseUp);
    };
  }, [isDragging, value, onChange, min, max]);

  return (
    <div className="flex flex-col items-center">
      <div 
        ref={knobRef}
        className="w-16 h-16 rounded-full relative cursor-ns-resize shadow-[inset_0_2px_4px_rgba(255,255,255,0.1),_0_4px_8px_rgba(0,0,0,0.5)] bg-gradient-to-b from-[#2a2a2a] to-[#1a1a1a] border border-[#111]"
        onMouseDown={() => setIsDragging(true)}
      >
        {/* Pointer indicator */}
        <div 
          className="absolute inset-0 flex items-start justify-center pt-2 pointer-events-none"
          style={{ transform: `rotate(${rotation}deg)` }}
        >
          <div 
            className="w-1.5 h-3 rounded-full" 
            style={{ backgroundColor: color, boxShadow: `0 0 5px ${color}` }} 
          />
        </div>
      </div>
      
      {/* Value display */}
      <div className="mt-3 bg-[#111] border border-[#222] px-3 py-1 rounded text-xs font-mono text-white text-center min-w-[3rem]">
        {Math.round(value)}
      </div>
    </div>
  );
};
