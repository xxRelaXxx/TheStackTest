<x-filament-panels::page>
    <form wire:submit.prevent="create" class="space-y-6">
        {{ $this->form }}
        
        <div class="fi-form-actions">
            <div class="flex flex-wrap items-center justify-end gap-3">
                <x-filament::button
                    type="submit"
                    size="lg"
                    wire:loading.attr="disabled"
                    icon="heroicon-o-check"
                >
                    Aggiungi
                </x-filament::button>
            </div>
        </div>
    </form>
    
    <x-filament-actions::modals />
</x-filament-panels::page>
