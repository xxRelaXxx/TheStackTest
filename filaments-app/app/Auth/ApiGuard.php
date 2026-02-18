<?php

namespace App\Auth;

use App\Models\User;
use App\Services\FinanceApiService;
use Illuminate\Contracts\Auth\Guard;
use Illuminate\Contracts\Auth\UserProvider;
use Illuminate\Support\Facades\Session;

class ApiGuard implements Guard
{
    protected $user;
    protected $provider;

    public function __construct(UserProvider $provider)
    {
        $this->provider = $provider;
        $this->user = null;
    }

    public function getProvider()
    {
        return $this->provider;
    }

    public function check()
    {
        return !is_null($this->user());
    }

    public function guest()
    {
        return !$this->check();
    }

    public function user()
    {
        if (!is_null($this->user)) {
            return $this->user;
        }

        $userData = Session::get('user');
        
        if ($userData) {
            $this->user = new User($userData);
        }

        return $this->user;
    }

    public function id()
    {
        if ($user = $this->user()) {
            return $user->getAuthIdentifier();
        }
    }

    public function validate(array $credentials = [])
    {
        try {
            $apiService = new FinanceApiService();
            $result = $apiService->login($credentials['email'], $credentials['password']);
            return !empty($result);
        } catch (\Exception $e) {
            return false;
        }
    }

    public function hasUser()
    {
        return !is_null($this->user);
    }

    public function setUser($user)
    {
        $this->user = $user;
        return $this;
    }

    public function attempt(array $credentials = [], $remember = false)
    {
        try {
            \Log::info('ApiGuard::attempt called', ['email' => $credentials['email'] ?? 'none']);
            
            $apiService = new FinanceApiService();
            $result = $apiService->login($credentials['email'], $credentials['password']);
            
            \Log::info('ApiGuard::attempt result', ['result' => $result]);
            
            if ($result && isset($result['user'])) {
                $this->user = new User($result['user']);
                Session::put('user', $result['user']);
                Session::put('api_token', $result['token']);
                return true;
            }
            
            return false;
        } catch (\Exception $e) {
            \Log::error('ApiGuard::attempt exception', ['message' => $e->getMessage(), 'trace' => $e->getTraceAsString()]);
            return false;
        }
    }

    public function attemptWhen(array $credentials = [], callable $callback = null, $remember = false)
    {
        try {
            \Log::info('ApiGuard::attemptWhen called', ['email' => $credentials['email'] ?? 'none']);
            
            $apiService = new FinanceApiService();
            $result = $apiService->login($credentials['email'], $credentials['password']);
            
            \Log::info('ApiGuard::attemptWhen result', ['result' => $result]);
            
            if ($result && isset($result['user'])) {
                $this->user = new User($result['user']);
                
                if ($callback && !$callback($this->user)) {
                    \Log::info('ApiGuard::attemptWhen callback returned false');
                    return false;
                }
                
                Session::put('user', $result['user']);
                Session::put('api_token', $result['token']);
                return true;
            }
            
            return false;
        } catch (\Exception $e) {
            \Log::error('ApiGuard::attemptWhen exception', ['message' => $e->getMessage()]);
            return false;
        }
    }

    public function once(array $credentials = [])
    {
        return $this->attempt($credentials);
    }

    public function login($user, $remember = false)
    {
        $this->user = $user;
    }

    public function loginUsingId($id, $remember = false)
    {
        return false;
    }

    public function onceUsingId($id)
    {
        return false;
    }

    public function viaRemember()
    {
        return false;
    }

    public function logout()
    {
        $apiService = new FinanceApiService();
        $apiService->logout();
        $this->user = null;
    }
}
