<x-filament-panels::page>
    <div class="space-y-6">
        <!-- Profile Header -->
        <div class="flex items-center justify-between">
            <div class="flex items-center space-x-4">
                <div class="w-16 h-16 rounded-full bg-blue-500 flex items-center justify-center text-white text-2xl font-bold">
                    A
                </div>
                <div>
                    <h2 class="text-2xl font-bold">Admin</h2>
                </div>
            </div>
        </div>

        <!-- Profile Form -->
        <form wire:submit="updateProfile">
            {{ $this->form }}
            
            <div class="mt-6">
                <x-filament::button type="submit">
                    Edit
                </x-filament::button>
            </div>
        </form>
    </div>
</x-filament-panels::page>
