import { useState, useEffect } from 'react';
import { Layout } from './components/Layout';
import { ChatPanel } from './components/ChatPanel';
import { SignalChain } from './components/SignalChain';
import { ParameterPanel } from './components/ParameterPanel';
import { PresetLibrary } from './components/PresetLibrary';
import { ChatMessage, Preset, SavedPreset } from './types';
import { api } from './services/api';

function App() {
  const [messages, setMessages] = useState<ChatMessage[]>([]);
  const [currentPreset, setCurrentPreset] = useState<Preset | null>(null);
  const [selectedBlockId, setSelectedBlockId] = useState<string | null>(null);
  const [savedPresets, setSavedPresets] = useState<SavedPreset[]>([]);
  const [isGenerating, setIsGenerating] = useState(false);

  // Initial load
  useEffect(() => {
    // In a real app, fetch catalog and presets here
    api.getPresets().then(setSavedPresets).catch(console.error);
  }, []);

  const handleSendMessage = async (content: string) => {
    const userMsg: ChatMessage = {
      id: Date.now().toString(),
      role: 'user',
      content,
      timestamp: new Date()
    };
    
    setMessages(prev => [...prev, userMsg]);
    setIsGenerating(true);

    try {
      const response = await api.generateTone({ prompt: content });
      
      const aiMsg: ChatMessage = {
        id: (Date.now() + 1).toString(),
        role: 'assistant',
        content: response.explanation + (response.warnings.length > 0 ? '\n\nWarnings: ' + response.warnings.join(', ') : ''),
        preset: response.preset,
        timestamp: new Date()
      };
      
      setMessages(prev => [...prev, aiMsg]);
      // Auto-load the generated preset
      setCurrentPreset(response.preset);
      setSelectedBlockId('amplifier'); // Default select amp
    } catch (error) {
      console.error('Failed to generate tone:', error);
      const errorMsg: ChatMessage = {
        id: (Date.now() + 1).toString(),
        role: 'assistant',
        content: 'Sorry, I encountered an error while generating your tone. Please make sure the backend server is running.',
        timestamp: new Date()
      };
      setMessages(prev => [...prev, errorMsg]);
    } finally {
      setIsGenerating(false);
    }
  };

  const handleParameterChange = (blockId: string, paramName: string, value: number) => {
    if (!currentPreset) return;
    
    setCurrentPreset(prev => {
      if (!prev) return prev;
      
      const updated = { ...prev };
      const block = (updated.sceneA as any)[blockId];
      
      if (blockId === 'amplifier') {
        block[paramName] = value;
      } else if (blockId === 'cabinet') {
        block[paramName] = value;
      } else {
        block.parameters[paramName] = value;
      }
      
      return updated;
    });
  };

  const handleToggleBlock = (blockId: string, enabled: boolean) => {
    if (!currentPreset) return;
    
    setCurrentPreset(prev => {
      if (!prev) return prev;
      const updated = { ...prev };
      if (blockId !== 'cabinet') {
        (updated.sceneA as any)[blockId].enabled = enabled;
      }
      return updated;
    });
  };

  const handleSavePreset = async () => {
    if (!currentPreset) return;
    
    try {
      const newSaved: SavedPreset = {
        id: Date.now().toString(),
        name: currentPreset.name || 'Custom Tone',
        originalPrompt: 'Manually saved',
        preset: currentPreset,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString()
      };
      
      // Attempt to save to backend, fallback to local state if it fails
      try {
        const saved = await api.savePreset(newSaved);
        setSavedPresets(prev => [...prev, saved]);
      } catch {
        setSavedPresets(prev => [...prev, newSaved]);
      }
    } catch (error) {
      console.error('Failed to save preset:', error);
    }
  };

  const handleDeletePreset = async (id: string) => {
    try {
      await api.deletePreset(id);
      setSavedPresets(prev => prev.filter(p => p.id !== id));
    } catch (error) {
      // Fallback for demo without backend
      setSavedPresets(prev => prev.filter(p => p.id !== id));
    }
  };

  const handleExportJson = async () => {
    if (!currentPreset) return;
    try {
      const json = await api.exportJson(currentPreset);
      const blob = new Blob([JSON.stringify(json, null, 2)], { type: 'application/json' });
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `${currentPreset.name.replace(/\\s+/g, '_')}.json`;
      a.click();
    } catch (error) {
      console.error('Export JSON failed:', error);
    }
  };

  const handleExportPatch = async () => {
    if (!currentPreset) return;
    try {
      const blob = await api.exportPatch(currentPreset);
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `${currentPreset.name.replace(/\\s+/g, '_')}.mg30patch`;
      a.click();
    } catch (error) {
      console.error('Export Patch failed:', error);
      alert('Failed to export patch. Check backend connection.');
    }
  };

  return (
    <Layout
      leftPanel={
        <ChatPanel 
          messages={messages} 
          onSendMessage={handleSendMessage}
          onLoadPreset={(p) => {
            setCurrentPreset(p);
            setSelectedBlockId('amplifier');
          }}
          isGenerating={isGenerating}
        />
      }
      centerPanel={
        <>
          <SignalChain 
            preset={currentPreset} 
            selectedBlockId={selectedBlockId}
            onSelectBlock={setSelectedBlockId}
            onToggleBlock={handleToggleBlock}
          />
          <ParameterPanel 
            preset={currentPreset}
            selectedBlockId={selectedBlockId}
            onParameterChange={handleParameterChange}
          />
        </>
      }
      rightPanel={
        <PresetLibrary 
          presets={savedPresets}
          currentPreset={currentPreset}
          onLoadPreset={setCurrentPreset}
          onDeletePreset={handleDeletePreset}
          onSavePreset={handleSavePreset}
          onExportJson={handleExportJson}
          onExportPatch={handleExportPatch}
        />
      }
    />
  );
}

export default App;
