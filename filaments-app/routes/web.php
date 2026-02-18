<?php

use Illuminate\Support\Facades\Route;
use App\Services\FinanceApiService;
use App\Auth\ApiGuard;
use App\Auth\ApiUserProvider;

Route::get('/', function () {
    return view('welcome');
});

Route::get('/test-api-login', function () {
    try {
        $service = new FinanceApiService();
        $result = $service->login('admin@gmail.com', 'admin123');
        return response()->json([
            'success' => true,
            'result' => $result
        ]);
    } catch (\Exception $e) {
        return response()->json([
            'success' => false,
            'error' => $e->getMessage(),
            'trace' => $e->getTraceAsString()
        ]);
    }
});

Route::get('/test-guard', function () {
    try {
        $guard = new ApiGuard(new ApiUserProvider());
        $result = $guard->attempt([
            'email' => 'admin@gmail.com',
            'password' => 'admin123'
        ]);
        
        return response()->json([
            'success' => $result,
            'user' => $guard->user()
        ]);
    } catch (\Exception $e) {
        return response()->json([
            'success' => false,
            'error' => $e->getMessage(),
            'trace' => $e->getTraceAsString()
        ]);
    }
});

Route::get('/test-filament-auth', function () {
    try {
        $guard = \Illuminate\Support\Facades\Auth::guard('api');
        
        return response()->json([
            'guard_class' => get_class($guard),
            'attempt' => $guard->attemptWhen([
                'email' => 'admin@gmail.com',
                'password' => 'admin123'
            ], function ($user) {
                return true;
            }),
            'user' => $guard->user()
        ]);
    } catch (\Exception $e) {
        return response()->json([
            'success' => false,
            'error' => $e->getMessage(),
            'trace' => $e->getTraceAsString()
        ]);
    }
});
