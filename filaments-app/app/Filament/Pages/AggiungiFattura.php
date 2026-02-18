<?php

namespace App\Filament\Pages;

use Filament\Pages\Page;
use Filament\Forms\Concerns\InteractsWithForms;
use Filament\Forms\Contracts\HasForms;
use Filament\Forms\Components\TextInput;
use Filament\Forms\Components\Select;
use Filament\Forms\Components\DatePicker;
use Filament\Schemas\Components\Section;
use Filament\Schemas\Schema;
use Filament\Schemas\Components\Utilities\Get;
use Filament\Schemas\Components\Utilities\Set;
use Filament\Actions\Action;
use Filament\Notifications\Notification;
use App\Services\FinanceApiService;
use BackedEnum;

class AggiungiFattura extends Page implements HasForms
{
    use InteractsWithForms;

    protected static string | BackedEnum | null $navigationIcon = 'heroicon-o-plus-circle';
    
    protected static ?string $navigationLabel = 'Inserisci Fattura';
    
    protected static ?int $navigationSort = 3;
    
    protected string $view = 'filament.pages.aggiungi-fattura';

    public ?array $data = [];

    public function mount(): void
    {
        $this->form->fill();
    }

    public function form(Schema $schema): Schema
    {
        $apiService = new FinanceApiService();

        return $schema
            ->schema([
                Section::make('Informazioni Cliente')
                    ->schema([
                        Select::make('clienteId')
                            ->label('Cliente')
                            ->required()
                            ->searchable()
                            ->options(function () use ($apiService) {
                                $clienti = $apiService->getClienti();
                                return collect($clienti)->pluck('nome', 'id')->toArray();
                            })
                            ->createOptionForm([
                                TextInput::make('nome')
                                    ->label('Nome')
                                    ->required(),
                                TextInput::make('codiceFiscale')
                                    ->label('Codice Fiscale'),
                                TextInput::make('partitaIVA')
                                    ->label('Partita IVA'),
                                TextInput::make('iban')
                                    ->label('IBAN'),
                                TextInput::make('email')
                                    ->label('Email')
                                    ->email(),
                                TextInput::make('telefono')
                                    ->label('Telefono'),
                                TextInput::make('indirizzo')
                                    ->label('Indirizzo'),
                            ])
                            ->createOptionUsing(function (array $data) use ($apiService) {
                                $cliente = $apiService->createCliente($data);
                                return $cliente['id'];
                            }),
                    ]),

                Section::make('Dettagli Fattura')
                    ->schema([
                        Select::make('tipoFattura')
                            ->label('Tipo di Fattura')
                            ->required()
                            ->options([
                                'Entrata' => 'Entrata',
                                'Uscita' => 'Uscita',
                            ])
                            ->default('Entrata'),

                        TextInput::make('numeroFattura')
                            ->label('Numero di Fattura')
                            ->required()
                            ->unique(ignoreRecord: true)
                            ->helperText('Se il numero esiste già, la fattura verrà sovrascritta'),

                        DatePicker::make('dataEmissione')
                            ->label('Data di Emissione')
                            ->required()
                            ->default(now())
                            ->native(false),

                        DatePicker::make('dataScadenza')
                            ->label('Scadenza Pagamento')
                            ->required()
                            ->default(now()->addDays(30))
                            ->native(false),
                    ])
                    ->columns(2),

                Section::make('Importi e Tasse')
                    ->schema([
                        TextInput::make('imponibileNetto')
                            ->label('Imponibile (Netto IVA)')
                            ->required()
                            ->numeric()
                            ->prefix('€')
                            ->live(onBlur: true)
                            ->afterStateUpdated(function (Get $get, Set $set, $state) use ($apiService) {
                                $assoggettamentoId = $get('assoggettamentoId');
                                if ($assoggettamentoId && $state) {
                                    $assoggettamenti = $apiService->getAssoggettamenti();
                                    $assoggettamento = collect($assoggettamenti)->firstWhere('id', $assoggettamentoId);
                                    if ($assoggettamento) {
                                        $percentuale = $assoggettamento['percentuale'];
                                        $totale = $state + ($state * $percentuale / 100);
                                        $set('totaleLordo', number_format($totale, 2, '.', ''));
                                    }
                                }
                            }),

                        Select::make('assoggettamentoId')
                            ->label('Assoggettamenti IVA')
                            ->required()
                            ->options(function () use ($apiService) {
                                $assoggettamenti = $apiService->getAssoggettamenti();
                                return collect($assoggettamenti)->mapWithKeys(function ($item) {
                                    return [$item['id'] => $item['nome'] . ' (' . $item['percentuale'] . '%)'];
                                })->toArray();
                            })
                            ->live(onBlur: true)
                            ->afterStateUpdated(function (Get $get, Set $set, $state) use ($apiService) {
                                $imponibile = $get('imponibileNetto');
                                if ($state && $imponibile) {
                                    $assoggettamenti = $apiService->getAssoggettamenti();
                                    $assoggettamento = collect($assoggettamenti)->firstWhere('id', $state);
                                    if ($assoggettamento) {
                                        $percentuale = $assoggettamento['percentuale'];
                                        $totale = $imponibile + ($imponibile * $percentuale / 100);
                                        $set('totaleLordo', number_format($totale, 2, '.', ''));
                                    }
                                }
                            }),

                        TextInput::make('totaleLordo')
                            ->label('Totale Fattura')
                            ->required()
                            ->numeric()
                            ->prefix('€')
                            ->disabled()
                            ->dehydrated(),
                    ])
                    ->columns(3),

                Section::make('Altre Informazioni')
                    ->schema([
                        Select::make('temaId')
                            ->label('Argomento (Tema)')
                            ->required()
                            ->options(function () use ($apiService) {
                                $temi = $apiService->getTemi();
                                return collect($temi)->pluck('nome', 'id')->toArray();
                            }),

                        Select::make('metodoPagamentoId')
                            ->label('Metodo di Pagamento')
                            ->required()
                            ->options(function () use ($apiService) {
                                $metodi = $apiService->getTipiPagamento();
                                return collect($metodi)->pluck('nome', 'id')->toArray();
                            }),

                        Select::make('valutaId')
                            ->label('Valuta')
                            ->required()
                            ->options(function () use ($apiService) {
                                $valute = $apiService->getValute();
                                return collect($valute)->mapWithKeys(function ($item) {
                                    return [$item['id'] => $item['nome'] . ' (' . $item['codice'] . ')'];
                                })->toArray();
                            })
                            ->default(1),

                        Select::make('arrotondamentoId')
                            ->label('Arrotondamenti')
                            ->options(function () use ($apiService) {
                                $arrotondamenti = $apiService->getArrotondamenti();
                                return collect($arrotondamenti)->pluck('nome', 'id')->toArray();
                            }),
                    ])
                    ->columns(2),
            ])
            ->statePath('data');
    }

    public function create(): void
    {
        $data = $this->form->getState();
        
        try {
            $apiService = new FinanceApiService();
            $apiService->createFattura($data);
            
            Notification::make()
                ->title('Fattura creata con successo')
                ->success()
                ->send();
            
            $this->redirect(route('filament.admin.pages.fatture'));
        } catch (\Exception $e) {
            Notification::make()
                ->title('Errore nella creazione della fattura')
                ->body($e->getMessage())
                ->danger()
                ->send();
        }
    }
}
