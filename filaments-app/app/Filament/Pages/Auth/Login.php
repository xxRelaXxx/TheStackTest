<?php

namespace App\Filament\Pages\Auth;

use Filament\Auth\Pages\Login as BaseLogin;
use Filament\Auth\Http\Responses\Contracts\LoginResponse;
use Illuminate\Support\Facades\Auth;
use Illuminate\Support\Facades\Log;

class Login extends BaseLogin
{
    public function authenticate(): ?LoginResponse
    {
        try {
            $data = $this->form->getState();
            
            Log::info('Custom Login::authenticate called', ['email' => $data['email'] ?? 'none']);
            
            $guard = Auth::guard('api');
            
            Log::info('Guard instance', ['class' => get_class($guard)]);
            
            $credentials = [
                'email' => $data['email'],
                'password' => $data['password'],
            ];
            
            $result = $guard->attemptWhen($credentials, function ($user) {
                Log::info('attemptWhen callback', ['user' => $user]);
                return true;
            });
            
            Log::info('Authentication result', ['result' => $result, 'user' => $guard->user()]);
            
            if (!$result) {
                $this->throwFailureValidationException();
            }
            
            session()->regenerate();
            
            return app(LoginResponse::class);
            
        } catch (\Exception $e) {
            Log::error('Login exception', ['message' => $e->getMessage(), 'trace' => $e->getTraceAsString()]);
            throw $e;
        }
    }
}
