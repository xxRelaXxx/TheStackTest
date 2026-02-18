<?php

namespace App\Filament\Pages;

use Filament\Pages\Page;
use Filament\Forms\Concerns\InteractsWithForms;
use Filament\Forms\Contracts\HasForms;
use Filament\Forms\Components\TextInput;
use Filament\Schemas\Components\Section;
use Filament\Forms\Components\Checkbox;
use Filament\Schemas\Schema;
use Filament\Notifications\Notification;
use Filament\Actions\Action as PageAction;
use App\Services\FinanceApiService;

class Profile extends Page implements HasForms
{
    use InteractsWithForms;

    protected string $view = 'filament.pages.profile';

    protected static bool $shouldRegisterNavigation = false;

    public ?array $data = [];
    public ?array $newAccountData = [];

    public function mount(): void
    {
        $apiService = new FinanceApiService();
        $user = $apiService->getUser();
        
        if ($user) {
            $this->form->fill([
                'username' => $user['username'],
                'email' => $user['email'],
            ]);
        }
    }

    public function form(Schema $schema): Schema
    {
        return $schema
            ->schema([
                Section::make('Personal Information')
                    ->schema([
                        TextInput::make('username')
                            ->label('Username')
                            ->required()
                            ->maxLength(255),

                        TextInput::make('email')
                            ->label('Email address')
                            ->email()
                            ->required()
                            ->maxLength(255),

                        TextInput::make('password')
                            ->label('Password')
                            ->password()
                            ->dehydrated(fn ($state) => filled($state))
                            ->required(fn (string $context): bool => $context === 'create')
                            ->maxLength(255)
                            ->helperText('Leave blank to keep current password'),
                    ])
                    ->columns(2),
            ])
            ->statePath('data');
    }

    public function updateProfile(): void
    {
        $data = $this->form->getState();
        $apiService = new FinanceApiService();
        $user = $apiService->getUser();
        
        try {
            $updateData = [
                'username' => $data['username'],
                'email' => $data['email'],
            ];
            
            if (!empty($data['password'])) {
                $updateData['password'] = $data['password'];
            }
            
            $apiService->updateAccount($user['id'], $updateData);
            
            Notification::make()
                ->title('Profile updated successfully')
                ->success()
                ->send();
        } catch (\Exception $e) {
            Notification::make()
                ->title('Error updating profile')
                ->body($e->getMessage())
                ->danger()
                ->send();
        }
    }

    protected function getHeaderActions(): array
    {
        $apiService = new FinanceApiService();
        $user = $apiService->getUser();
        
        if (!$user || !$user['isAdmin']) {
            return [];
        }

        return [
            PageAction::make('addAccount')
                ->label('Aggiungi Account')
                ->icon('heroicon-o-plus')
                ->form([
                    TextInput::make('username')
                        ->label('Username')
                        ->required()
                        ->maxLength(255),

                    TextInput::make('email')
                        ->label('Email')
                        ->email()
                        ->required()
                        ->maxLength(255),

                    TextInput::make('password')
                        ->label('Password')
                        ->password()
                        ->required()
                        ->maxLength(255),

                    Checkbox::make('isAdmin')
                        ->label('Admin')
                        ->default(false),
                ])
                ->action(function (array $data) use ($apiService) {
                    try {
                        $apiService->createAccount($data);
                        
                        Notification::make()
                            ->title('Account created successfully')
                            ->success()
                            ->send();
                    } catch (\Exception $e) {
                        Notification::make()
                            ->title('Error creating account')
                            ->body($e->getMessage())
                            ->danger()
                            ->send();
                    }
                }),

            PageAction::make('logout')
                ->label('Logout')
                ->icon('heroicon-o-arrow-right-on-rectangle')
                ->color('danger')
                ->action(function () use ($apiService) {
                    $apiService->logout();
                    redirect()->route('filament.admin.auth.login');
                }),
        ];
    }

    protected function getFormActions(): array
    {
        return [
            PageAction::make('save')
                ->label('Edit')
                ->submit('updateProfile'),
        ];
    }
}
