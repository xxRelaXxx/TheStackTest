<?php

namespace App\Filament\Pages;

use Filament\Pages\Page;
use Filament\Forms\Components\Select;
use Filament\Actions\Action;
use Filament\Notifications\Notification;
use App\Services\FinanceApiService;
use BackedEnum;
use Livewire\Attributes\On;

class Fatture extends Page
{
    protected static string | BackedEnum | null $navigationIcon = 'heroicon-o-document-text';
    
    protected static ?string $navigationLabel = 'Fatture';
    
    protected static ?int $navigationSort = 2;
    
    protected string $view = 'filament.pages.fatture-simple';

    public array $entrate = [];
    public array $uscite = [];
    public ?string $tipoFilter = null;
    public ?int $clienteFilter = null;
    public ?int $temaFilter = null;

    public function mount(): void
    {
        $this->loadData();
    }

    public function loadData(): void
    {
        $apiService = new FinanceApiService();
        $allFatture = $apiService->getFatture($this->tipoFilter, $this->clienteFilter, $this->temaFilter);
        
        $this->entrate = array_filter($allFatture, fn($f) => $f['tipoFattura'] === 'Entrata');
        $this->uscite = array_filter($allFatture, fn($f) => $f['tipoFattura'] === 'Uscita');
    }

    public function deleteFattura(int $id): void
    {
        $apiService = new FinanceApiService();
        $apiService->deleteFattura($id);
        
        Notification::make()
            ->title('Fattura eliminata')
            ->success()
            ->send();
        
        $this->loadData();
    }

    protected function getHeaderActions(): array
    {
        return [
            Action::make('filter')
                ->label('Filtra')
                ->icon('heroicon-o-funnel')
                ->form([
                    Select::make('tipo')
                        ->label('Tipo Fattura')
                        ->options([
                            'Entrata' => 'Entrata',
                            'Uscita' => 'Uscita',
                        ])
                        ->placeholder('Tutte'),
                    
                    Select::make('cliente')
                        ->label('Cliente')
                        ->options(function () {
                            $apiService = new FinanceApiService();
                            $clienti = $apiService->getClienti();
                            return collect($clienti)->pluck('nome', 'id')->toArray();
                        })
                        ->searchable()
                        ->placeholder('Tutti'),
                    
                    Select::make('tema')
                        ->label('Argomento')
                        ->options(function () {
                            $apiService = new FinanceApiService();
                            $temi = $apiService->getTemi();
                            return collect($temi)->pluck('nome', 'id')->toArray();
                        })
                        ->searchable()
                        ->placeholder('Tutti'),
                ])
                ->action(function (array $data) {
                    $this->tipoFilter = $data['tipo'] ?? null;
                    $this->clienteFilter = $data['cliente'] ?? null;
                    $this->temaFilter = $data['tema'] ?? null;
                    $this->loadData();
                }),
            
            Action::make('add')
                ->label('Aggiungi Fattura')
                ->icon('heroicon-o-plus')
                ->url(route('filament.admin.pages.aggiungi-fattura')),
        ];
    }
}
