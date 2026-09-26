import React from 'react';

interface LayoutProps {
  leftPanel: React.ReactNode;
  centerPanel: React.ReactNode;
  rightPanel: React.ReactNode;
}

export const Layout: React.FC<LayoutProps> = ({ leftPanel, centerPanel, rightPanel }) => {
  return (
    <div className="flex flex-col h-screen bg-[#0f0f0f] text-[#eee] overflow-hidden">
      <header className="h-16 flex items-center px-6 border-b border-[#1a1a2e] bg-[#1a1a2e]/50">
        <h1 className="text-xl font-bold bg-gradient-to-r from-[#e94560] to-[#533483] bg-clip-text text-transparent">
          NUX MG-30 AI Tone Generator
        </h1>
      </header>
      
      <main className="flex-1 flex overflow-hidden">
        <aside className="w-[350px] border-r border-[#1a1a2e] bg-[#16213e]/30 flex flex-col">
          {leftPanel}
        </aside>
        
        <section className="flex-1 flex flex-col relative overflow-hidden bg-[#0f0f0f]">
          {centerPanel}
        </section>
        
        <aside className="w-[300px] border-l border-[#1a1a2e] bg-[#16213e]/30 flex flex-col">
          {rightPanel}
        </aside>
      </main>
    </div>
  );
};
