import React, { useState, useRef, useEffect } from 'react';
import { Send, Loader2, Play } from 'lucide-react';
import { ChatMessage, Preset } from '../types';

interface ChatPanelProps {
  messages: ChatMessage[];
  onSendMessage: (content: string) => void;
  onLoadPreset: (preset: Preset) => void;
  isGenerating: boolean;
}

const EXAMPLE_PROMPTS = [
  "David Gilmour solo tone",
  "Metallica heavy rhythm",
  "John Mayer clean blues",
  "Jimi Hendrix fuzz lead",
  "Country chicken pickin'"
];

export const ChatPanel: React.FC<ChatPanelProps> = ({ 
  messages, 
  onSendMessage, 
  onLoadPreset,
  isGenerating 
}) => {
  const [input, setInput] = useState('');
  const messagesEndRef = useRef<HTMLDivElement>(null);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  useEffect(() => {
    scrollToBottom();
  }, [messages, isGenerating]);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (input.trim() && !isGenerating) {
      onSendMessage(input.trim());
      setInput('');
    }
  };

  return (
    <div className="flex flex-col h-full relative">
      <div className="flex-1 overflow-y-auto p-4 space-y-6">
        {messages.length === 0 ? (
          <div className="flex flex-col items-center justify-center h-full text-center space-y-6">
            <div className="text-[#aaa]">
              <p className="mb-4">Describe the tone you want to create.</p>
              <p className="text-sm">Try one of these examples:</p>
            </div>
            <div className="flex flex-col gap-2 w-full max-w-[250px]">
              {EXAMPLE_PROMPTS.map((prompt) => (
                <button
                  key={prompt}
                  onClick={() => onSendMessage(prompt)}
                  className="px-4 py-2 text-sm rounded-lg bg-[#1a1a2e] hover:bg-[#16213e] border border-[#533483]/30 transition-colors text-left text-[#eee]"
                >
                  "{prompt}"
                </button>
              ))}
            </div>
          </div>
        ) : (
          <>
            {messages.map((msg) => (
              <div 
                key={msg.id} 
                className={`flex flex-col ${msg.role === 'user' ? 'items-end' : 'items-start'}`}
              >
                <div 
                  className={`max-w-[85%] rounded-2xl px-4 py-3 ${
                    msg.role === 'user' 
                      ? 'bg-[#533483] text-white rounded-tr-sm' 
                      : 'bg-[#1a1a2e] text-[#eee] rounded-tl-sm border border-[#16213e]'
                  }`}
                >
                  <p className="whitespace-pre-wrap text-sm leading-relaxed">{msg.content}</p>
                </div>
                
                {msg.preset && (
                  <button
                    onClick={() => onLoadPreset(msg.preset!)}
                    className="mt-2 flex items-center gap-2 px-4 py-2 bg-[#e94560]/10 text-[#e94560] border border-[#e94560]/30 rounded-lg hover:bg-[#e94560]/20 transition-colors text-sm font-medium"
                  >
                    <Play size={14} />
                    Load Preset
                  </button>
                )}
              </div>
            ))}
            
            {isGenerating && (
              <div className="flex items-start">
                <div className="bg-[#1a1a2e] rounded-2xl rounded-tl-sm px-4 py-3 border border-[#16213e] flex items-center gap-3">
                  <Loader2 className="animate-spin text-[#e94560]" size={18} />
                  <span className="text-sm text-[#aaa]">Generating tone...</span>
                </div>
              </div>
            )}
            <div ref={messagesEndRef} />
          </>
        )}
      </div>

      <div className="p-4 bg-[#16213e]/50 border-t border-[#1a1a2e]">
        <form onSubmit={handleSubmit} className="relative">
          <input
            type="text"
            value={input}
            onChange={(e) => setInput(e.target.value)}
            placeholder="Describe your dream tone..."
            className="w-full bg-[#0f0f0f] border border-[#533483]/50 rounded-xl py-3 pl-4 pr-12 text-sm focus:outline-none focus:border-[#e94560] transition-colors text-white placeholder-[#aaa]"
            disabled={isGenerating}
          />
          <button
            type="submit"
            disabled={!input.trim() || isGenerating}
            className="absolute right-2 top-1/2 -translate-y-1/2 p-2 text-[#e94560] hover:text-white disabled:text-[#333] disabled:hover:text-[#333] transition-colors"
          >
            <Send size={18} />
          </button>
        </form>
      </div>
    </div>
  );
};
