<?php

namespace App\Models;

use Filament\Models\Contracts\FilamentUser;
use Filament\Panel;
use Illuminate\Foundation\Auth\User as Authenticatable;

class User extends Authenticatable implements FilamentUser
{
    protected $fillable = [
        'id',
        'username',
        'email',
        'isAdmin',
        'name',
    ];

    protected $hidden = [];
    
    public $incrementing = false;
    public $timestamps = false;

    public function __construct(array $attributes = [])
    {
        parent::__construct($attributes);
        
        // Set attributes directly from API response
        foreach ($attributes as $key => $value) {
            $this->setAttribute($key, $value);
        }
        
        // Set name from username if not provided
        if (!isset($attributes['name'])) {
            $this->setAttribute('name', $attributes['username'] ?? $attributes['email'] ?? 'User');
        }
    }

    public function canAccessPanel(Panel $panel): bool
    {
        return true;
    }
    
    public function getFilamentName(): string
    {
        return $this->getAttribute('name') 
            ?? $this->getAttribute('username') 
            ?? $this->getAttribute('email') 
            ?? 'User';
    }

    public function getAuthIdentifier()
    {
        return $this->getAttribute('id');
    }

    public function getAuthIdentifierName()
    {
        return 'id';
    }

    // Override to prevent database queries
    public function getAuthPassword()
    {
        return null;
    }
    
    public function getAuthPasswordName()
    {
        return null;
    }
}
