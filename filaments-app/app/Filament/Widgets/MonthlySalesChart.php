<?php

namespace App\Filament\Widgets;

use Filament\Widgets\ChartWidget;
use App\Services\FinanceApiService;

class MonthlySalesChart extends ChartWidget
{
    protected ?string $heading = 'Monthly Sales';

    protected static ?int $sort = 2;

    protected int | string | array $columnSpan = 'full';

    protected function getData(): array
    {
        $apiService = new FinanceApiService();
        $stats = $apiService->getDashboardStats();
        
        $monthlyData = $stats['ultimiDodiciMesi'] ?? [];
        
        $labels = [];
        $data = [];
        
        foreach ($monthlyData as $month) {
            $labels[] = $month['mese'] ?? '';
            $data[] = $month['valore'] ?? 0;
        }

        return [
            'datasets' => [
                [
                    'label' => 'Sales',
                    'data' => $data,
                    'backgroundColor' => 'rgba(59, 130, 246, 0.5)',
                    'borderColor' => 'rgb(59, 130, 246)',
                ],
            ],
            'labels' => $labels,
        ];
    }

    protected function getType(): string
    {
        return 'bar';
    }

    protected function getOptions(): array
    {
        return [
            'plugins' => [
                'legend' => [
                    'display' => false,
                ],
            ],
            'scales' => [
                'y' => [
                    'beginAtZero' => true,
                ],
            ],
        ];
    }
}
