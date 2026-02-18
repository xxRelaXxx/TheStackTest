<?php

namespace App\Filament\Widgets;

use Filament\Widgets\StatsOverviewWidget as BaseWidget;
use Filament\Widgets\StatsOverviewWidget\Stat;
use App\Services\FinanceApiService;

class StatsOverview extends BaseWidget
{
    protected function getStats(): array
    {
        $apiService = new FinanceApiService();
        $stats = $apiService->getDashboardStats();

        $profitto = $stats['profitto'] ?? 0;
        $percentualeProfitto = $stats['percentualeProfitto'] ?? 0;
        $entrate = $stats['entrate'] ?? 0;
        $percentualeEntrate = $stats['percentualeEntrate'] ?? 0;

        return [
            Stat::make('Profitto', number_format($profitto, 2, ',', '.') . ' €')
                ->description($this->getPercentageDescription($percentualeProfitto))
                ->descriptionIcon($percentualeProfitto >= 0 ? 'heroicon-m-arrow-trending-up' : 'heroicon-m-arrow-trending-down')
                ->color($percentualeProfitto >= 0 ? 'success' : 'danger')
                ->chart($this->generateSparkline($profitto)),
            
            Stat::make('Entrate', number_format($entrate, 2, ',', '.') . ' €')
                ->description($this->getPercentageDescription($percentualeEntrate))
                ->descriptionIcon($percentualeEntrate >= 0 ? 'heroicon-m-arrow-trending-up' : 'heroicon-m-arrow-trending-down')
                ->color($percentualeEntrate >= 0 ? 'success' : 'danger')
                ->chart($this->generateSparkline($entrate)),
        ];
    }

    protected function getPercentageDescription(float $percentage): string
    {
        $abs = abs($percentage);
        $formatted = number_format($abs, 2, ',', '.');
        $sign = $percentage >= 0 ? '+' : '';
        return "{$sign}{$formatted}%";
    }

    protected function generateSparkline(float $value): array
    {
        // Generate a simple sparkline for visual effect
        $base = $value > 0 ? $value * 0.8 : 0;
        return [
            $base,
            $base * 1.1,
            $base * 0.95,
            $base * 1.15,
            $base * 1.05,
            $value,
        ];
    }

    protected function getColumns(): int
    {
        return 2;
    }
}
