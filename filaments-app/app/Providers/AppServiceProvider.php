<?php

namespace App\Providers;

use Illuminate\Support\ServiceProvider;
use Illuminate\Support\Facades\Auth;
use App\Auth\ApiGuard;
use App\Auth\ApiUserProvider;

class AppServiceProvider extends ServiceProvider
{
    /**
     * Register any application services.
     */
    public function register(): void
    {
        //
    }

    /**
     * Bootstrap any application services.
     */
    public function boot(): void
    {
        // Register custom API auth driver
        Auth::extend('api-guard', function ($app, $name, array $config) {
            return new ApiGuard(new ApiUserProvider());
        });
    }
}
