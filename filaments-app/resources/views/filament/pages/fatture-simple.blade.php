<x-filament-panels::page>
    <div class="space-y-6">
        <!-- Entrate Section -->
        <div class="fi-section rounded-xl bg-white shadow-sm ring-1 ring-gray-950/5 dark:bg-gray-900 dark:ring-white/10">
            <div class="fi-section-header flex flex-col gap-3 px-6 py-4">
                <div class="flex items-center gap-3">
                    <div class="grid flex-1 gap-y-1">
                        <h3 class="fi-section-header-heading text-base font-semibold leading-6 text-gray-950 dark:text-white">
                            Fatture in Entrata
                        </h3>
                    </div>
                </div>
            </div>

            <div class="fi-section-content-ctn border-t border-gray-200 dark:border-white/10">
                <div class="overflow-hidden">
                    <div class="overflow-x-auto">
                        <table class="fi-ta-table w-full table-auto divide-y divide-gray-200 text-start dark:divide-white/5">
                            <thead class="divide-y divide-gray-200 dark:divide-white/5">
                                <tr class="bg-gray-50 dark:bg-white/5">
                                    <th class="fi-ta-header-cell px-3 py-3.5 sm:first-of-type:ps-6 sm:last-of-type:pe-6">
                                        <span class="text-sm font-semibold text-gray-950 dark:text-white">N. Fattura</span>
                                    </th>
                                    <th class="fi-ta-header-cell px-3 py-3.5">
                                        <span class="text-sm font-semibold text-gray-950 dark:text-white">Cliente</span>
                                    </th>
                                    <th class="fi-ta-header-cell px-3 py-3.5">
                                        <span class="text-sm font-semibold text-gray-950 dark:text-white">Tema</span>
                                    </th>
                                    <th class="fi-ta-header-cell px-3 py-3.5">
                                        <span class="text-sm font-semibold text-gray-950 dark:text-white">Netto</span>
                                    </th>
                                    <th class="fi-ta-header-cell px-3 py-3.5">
                                        <span class="text-sm font-semibold text-gray-950 dark:text-white">Totale</span>
                                    </th>
                                    <th class="fi-ta-header-cell px-3 py-3.5">
                                        <span class="text-sm font-semibold text-gray-950 dark:text-white">Scadenza</span>
                                    </th>
                                    <th class="fi-ta-header-cell px-3 py-3.5 sm:last-of-type:pe-6">
                                        <span class="text-sm font-semibold text-gray-950 dark:text-white">Azioni</span>
                                    </th>
                                </tr>
                            </thead>
                            <tbody class="divide-y divide-gray-200 whitespace-nowrap dark:divide-white/5">
                                @forelse($entrate as $fattura)
                                    <tr class="fi-ta-row hover:bg-gray-50 dark:hover:bg-white/5">
                                        <td class="fi-ta-cell p-0 first-of-type:ps-1 last-of-type:pe-1 sm:first-of-type:ps-3 sm:last-of-type:pe-3">
                                            <div class="fi-ta-col-wrp px-3 py-4">
                                                <div class="text-sm text-gray-950 dark:text-white">
                                                    {{ $fattura['numeroFattura'] ?? 'N/A' }}
                                                </div>
                                            </div>
                                        </td>
                                        <td class="fi-ta-cell p-0">
                                            <div class="fi-ta-col-wrp px-3 py-4">
                                                <div class="text-sm text-gray-950 dark:text-white">
                                                    {{ $fattura['cliente']['nome'] ?? 'N/A' }}
                                                </div>
                                            </div>
                                        </td>
                                        <td class="fi-ta-cell p-0">
                                            <div class="fi-ta-col-wrp px-3 py-4">
                                                <span class="fi-badge flex items-center justify-center gap-x-1 rounded-md text-xs font-medium ring-1 ring-inset px-2 min-w-[theme(spacing.6)] py-1 fi-badge-size-md bg-primary-50 text-primary-600 ring-primary-600/10 dark:bg-primary-400/10 dark:text-primary-400 dark:ring-primary-400/30">
                                                    {{ $fattura['tema']['nome'] ?? 'N/A' }}
                                                </span>
                                            </div>
                                        </td>
                                        <td class="fi-ta-cell p-0">
                                            <div class="fi-ta-col-wrp px-3 py-4">
                                                <div class="text-sm text-gray-950 dark:text-white">
                                                    €{{ number_format($fattura['imponibileNetto'] ?? 0, 2, ',', '.') }}
                                                </div>
                                            </div>
                                        </td>
                                        <td class="fi-ta-cell p-0">
                                            <div class="fi-ta-col-wrp px-3 py-4">
                                                <div class="text-sm font-medium text-gray-950 dark:text-white">
                                                    €{{ number_format($fattura['totaleLordo'] ?? 0, 2, ',', '.') }}
                                                </div>
                                            </div>
                                        </td>
                                        <td class="fi-ta-cell p-0">
                                            <div class="fi-ta-col-wrp px-3 py-4">
                                                <div class="text-sm text-gray-950 dark:text-white">
                                                    @if(isset($fattura['dataScadenza']))
                                                        {{ \Carbon\Carbon::parse($fattura['dataScadenza'])->format('d/m/Y') }}
                                                    @else
                                                        N/A
                                                    @endif
                                                </div>
                                            </div>
                                        </td>
                                        <td class="fi-ta-cell p-0 sm:last-of-type:pe-3">
                                            <div class="fi-ta-col-wrp px-3 py-4">
                                                <button 
                                                    wire:click="deleteFattura({{ $fattura['id'] }})"
                                                    wire:confirm="Sei sicuro di voler eliminare questa fattura?"
                                                    class="fi-icon-btn relative flex items-center justify-center rounded-lg outline-none transition duration-75 focus-visible:ring-2 disabled:pointer-events-none disabled:opacity-70 h-9 w-9 text-danger-600 hover:text-danger-500 dark:text-danger-500 dark:hover:text-danger-400 fi-color-danger"
                                                    type="button">
                                                    <svg class="fi-icon-btn-icon h-5 w-5" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
                                                        <path fill-rule="evenodd" d="M8.75 1A2.75 2.75 0 006 3.75v.443c-.795.077-1.584.176-2.365.298a.75.75 0 10.23 1.482l.149-.022.841 10.518A2.75 2.75 0 007.596 19h4.807a2.75 2.75 0 002.742-2.53l.841-10.52.149.023a.75.75 0 00.23-1.482A41.03 41.03 0 0014 4.193V3.75A2.75 2.75 0 0011.25 1h-2.5zM10 4c.84 0 1.673.025 2.5.075V3.75c0-.69-.56-1.25-1.25-1.25h-2.5c-.69 0-1.25.56-1.25 1.25v.325C8.327 4.025 9.16 4 10 4zM8.58 7.72a.75.75 0 00-1.5.06l.3 7.5a.75.75 0 101.5-.06l-.3-7.5zm4.34.06a.75.75 0 10-1.5-.06l-.3 7.5a.75.75 0 101.5.06l.3-7.5z" clip-rule="evenodd"></path>
                                                    </svg>
                                                </button>
                                            </div>
                                        </td>
                                    </tr>
                                @empty
                                    <tr>
                                        <td colspan="7" class="fi-ta-empty-state-content px-6 py-12">
                                            <div class="fi-ta-empty-state-heading text-center text-sm font-semibold text-gray-500 dark:text-gray-400">
                                                Nessuna fattura in entrata
                                            </div>
                                        </td>
                                    </tr>
                                @endforelse
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>

        <!-- Uscite Section -->
        <div class="fi-section rounded-xl bg-white shadow-sm ring-1 ring-gray-950/5 dark:bg-gray-900 dark:ring-white/10">
            <div class="fi-section-header flex flex-col gap-3 px-6 py-4">
                <div class="flex items-center gap-3">
                    <div class="grid flex-1 gap-y-1">
                        <h3 class="fi-section-header-heading text-base font-semibold leading-6 text-gray-950 dark:text-white">
                            Fatture in Uscita
                        </h3>
                    </div>
                </div>
            </div>

            <div class="fi-section-content-ctn border-t border-gray-200 dark:border-white/10">
                <div class="overflow-hidden">
                    <div class="overflow-x-auto">
                        <table class="fi-ta-table w-full table-auto divide-y divide-gray-200 text-start dark:divide-white/5">
                            <thead class="divide-y divide-gray-200 dark:divide-white/5">
                                <tr class="bg-gray-50 dark:bg-white/5">
                                    <th class="fi-ta-header-cell px-3 py-3.5 sm:first-of-type:ps-6 sm:last-of-type:pe-6">
                                        <span class="text-sm font-semibold text-gray-950 dark:text-white">N. Fattura</span>
                                    </th>
                                    <th class="fi-ta-header-cell px-3 py-3.5">
                                        <span class="text-sm font-semibold text-gray-950 dark:text-white">Cliente</span>
                                    </th>
                                    <th class="fi-ta-header-cell px-3 py-3.5">
                                        <span class="text-sm font-semibold text-gray-950 dark:text-white">Tema</span>
                                    </th>
                                    <th class="fi-ta-header-cell px-3 py-3.5">
                                        <span class="text-sm font-semibold text-gray-950 dark:text-white">Netto</span>
                                    </th>
                                    <th class="fi-ta-header-cell px-3 py-3.5">
                                        <span class="text-sm font-semibold text-gray-950 dark:text-white">Totale</span>
                                    </th>
                                    <th class="fi-ta-header-cell px-3 py-3.5">
                                        <span class="text-sm font-semibold text-gray-950 dark:text-white">Scadenza</span>
                                    </th>
                                    <th class="fi-ta-header-cell px-3 py-3.5 sm:last-of-type:pe-6">
                                        <span class="text-sm font-semibold text-gray-950 dark:text-white">Azioni</span>
                                    </th>
                                </tr>
                            </thead>
                            <tbody class="divide-y divide-gray-200 whitespace-nowrap dark:divide-white/5">
                                @forelse($uscite as $fattura)
                                    <tr class="fi-ta-row hover:bg-gray-50 dark:hover:bg-white/5">
                                        <td class="fi-ta-cell p-0 first-of-type:ps-1 last-of-type:pe-1 sm:first-of-type:ps-3 sm:last-of-type:pe-3">
                                            <div class="fi-ta-col-wrp px-3 py-4">
                                                <div class="text-sm text-gray-950 dark:text-white">
                                                    {{ $fattura['numeroFattura'] ?? 'N/A' }}
                                                </div>
                                            </div>
                                        </td>
                                        <td class="fi-ta-cell p-0">
                                            <div class="fi-ta-col-wrp px-3 py-4">
                                                <div class="text-sm text-gray-950 dark:text-white">
                                                    {{ $fattura['cliente']['nome'] ?? 'N/A' }}
                                                </div>
                                            </div>
                                        </td>
                                        <td class="fi-ta-cell p-0">
                                            <div class="fi-ta-col-wrp px-3 py-4">
                                                <span class="fi-badge flex items-center justify-center gap-x-1 rounded-md text-xs font-medium ring-1 ring-inset px-2 min-w-[theme(spacing.6)] py-1 fi-badge-size-md bg-danger-50 text-danger-600 ring-danger-600/10 dark:bg-danger-400/10 dark:text-danger-400 dark:ring-danger-400/30">
                                                    {{ $fattura['tema']['nome'] ?? 'N/A' }}
                                                </span>
                                            </div>
                                        </td>
                                        <td class="fi-ta-cell p-0">
                                            <div class="fi-ta-col-wrp px-3 py-4">
                                                <div class="text-sm text-gray-950 dark:text-white">
                                                    €{{ number_format($fattura['imponibileNetto'] ?? 0, 2, ',', '.') }}
                                                </div>
                                            </div>
                                        </td>
                                        <td class="fi-ta-cell p-0">
                                            <div class="fi-ta-col-wrp px-3 py-4">
                                                <div class="text-sm font-medium text-gray-950 dark:text-white">
                                                    €{{ number_format($fattura['totaleLordo'] ?? 0, 2, ',', '.') }}
                                                </div>
                                            </div>
                                        </td>
                                        <td class="fi-ta-cell p-0">
                                            <div class="fi-ta-col-wrp px-3 py-4">
                                                <div class="text-sm text-gray-950 dark:text-white">
                                                    @if(isset($fattura['dataScadenza']))
                                                        {{ \Carbon\Carbon::parse($fattura['dataScadenza'])->format('d/m/Y') }}
                                                    @else
                                                        N/A
                                                    @endif
                                                </div>
                                            </div>
                                        </td>
                                        <td class="fi-ta-cell p-0 sm:last-of-type:pe-3">
                                            <div class="fi-ta-col-wrp px-3 py-4">
                                                <button 
                                                    wire:click="deleteFattura({{ $fattura['id'] }})"
                                                    wire:confirm="Sei sicuro di voler eliminare questa fattura?"
                                                    class="fi-icon-btn relative flex items-center justify-center rounded-lg outline-none transition duration-75 focus-visible:ring-2 disabled:pointer-events-none disabled:opacity-70 h-9 w-9 text-danger-600 hover:text-danger-500 dark:text-danger-500 dark:hover:text-danger-400 fi-color-danger"
                                                    type="button">
                                                    <svg class="fi-icon-btn-icon h-5 w-5" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
                                                        <path fill-rule="evenodd" d="M8.75 1A2.75 2.75 0 006 3.75v.443c-.795.077-1.584.176-2.365.298a.75.75 0 10.23 1.482l.149-.022.841 10.518A2.75 2.75 0 007.596 19h4.807a2.75 2.75 0 002.742-2.53l.841-10.52.149.023a.75.75 0 00.23-1.482A41.03 41.03 0 0014 4.193V3.75A2.75 2.75 0 0011.25 1h-2.5zM10 4c.84 0 1.673.025 2.5.075V3.75c0-.69-.56-1.25-1.25-1.25h-2.5c-.69 0-1.25.56-1.25 1.25v.325C8.327 4.025 9.16 4 10 4zM8.58 7.72a.75.75 0 00-1.5.06l.3 7.5a.75.75 0 101.5-.06l-.3-7.5zm4.34.06a.75.75 0 10-1.5-.06l-.3 7.5a.75.75 0 101.5.06l.3-7.5z" clip-rule="evenodd"></path>
                                                    </svg>
                                                </button>
                                            </div>
                                        </td>
                                    </tr>
                                @empty
                                    <tr>
                                        <td colspan="7" class="fi-ta-empty-state-content px-6 py-12">
                                            <div class="fi-ta-empty-state-heading text-center text-sm font-semibold text-gray-500 dark:text-gray-400">
                                                Nessuna fattura in uscita
                                            </div>
                                        </td>
                                    </tr>
                                @endforelse
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>
</x-filament-panels::page>
