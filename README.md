# ColumnStore

Time-series column store (Key as time)
Data compressed and stored via https://github.com/DBalashov/FileContainer in memory or to file

## Benchmark

|        Method | Compressed | PageSize |        Mean |        Error |    StdDev | Ratio | RatioSD | Avg MB/sec | Bytes/value |
|-------------- |----------- |--------- |------------:|-------------:|----------:|------:|--------:|----------- |------------ |
| EntitiesMerge |      False |     2048 | 2,704.84 ms |   577.435 ms | 31.651 ms |  1.00 |    0.00 |      77.03 |           - |
| EntitiesMerge |       True |     2048 | 5,532.74 ms |   302.760 ms | 16.595 ms |  1.00 |    0.00 |       9.93 |           - |
| EntitiesMerge |      False |     4096 | 2,674.62 ms | 1,123.777 ms | 61.598 ms |  1.00 |    0.00 |      78.09 |           - |
| EntitiesMerge |       True |     4096 | 5,508.66 ms |   274.641 ms | 15.054 ms |  1.00 |    0.00 |      10.07 |           - |
| EntitiesMerge |      False |     8192 | 2,528.02 ms | 1,641.312 ms | 89.966 ms |  1.00 |    0.00 |      83.07 |           - |
| EntitiesMerge |       True |     8192 | 5,499.24 ms |   474.825 ms | 26.027 ms |  1.00 |    0.00 |      10.37 |           - |
| EntitiesMerge |      False |    16384 | 2,655.22 ms |   757.487 ms | 41.520 ms |  1.00 |    0.00 |      79.68 |           - |
| EntitiesMerge |       True |    16384 | 5,454.52 ms |   199.181 ms | 10.918 ms |  1.00 |    0.00 |      10.99 |           - |
|  EntitiesRead |      False |     2048 |   442.92 ms |   165.160 ms |  9.053 ms |  0.16 |    0.01 |      69.92 |         7.7 |
|  EntitiesRead |       True |     2048 |   528.02 ms |   175.466 ms |  9.618 ms |  0.10 |    0.00 |      13.38 |         1.8 |
|  EntitiesRead |      False |     4096 |   422.35 ms |   160.310 ms |  8.787 ms |  0.16 |    0.01 |      73.47 |         7.7 |
|  EntitiesRead |       True |     4096 |   521.81 ms |   108.601 ms |  5.953 ms |  0.09 |    0.00 |      13.67 |         1.8 |
|  EntitiesRead |      False |     8192 |   418.53 ms |   234.476 ms | 12.852 ms |  0.17 |    0.01 |      74.50 |         7.8 |
|  EntitiesRead |       True |     8192 |   526.27 ms |    76.214 ms |  4.178 ms |  0.10 |    0.00 |      13.84 |         1.8 |
|  EntitiesRead |      False |    16384 |   446.49 ms |   280.148 ms | 15.356 ms |  0.17 |    0.01 |      70.41 |         7.8 |
|  EntitiesRead |       True |    16384 |   557.59 ms |   588.091 ms | 32.235 ms |  0.10 |    0.01 |      13.82 |         1.9 |
| EntitiesWrite |      False |     2048 |   916.99 ms |   175.707 ms |  9.631 ms |  0.34 |    0.01 |     197.62 |           - |
| EntitiesWrite |       True |     2048 | 3,180.21 ms |   276.593 ms | 15.161 ms |  0.57 |    0.00 |      15.24 |           - |
| EntitiesWrite |      False |     4096 |   914.86 ms |   206.765 ms | 11.333 ms |  0.34 |    0.01 |     198.54 |           - |
| EntitiesWrite |       True |     4096 | 3,188.99 ms |   165.663 ms |  9.081 ms |  0.58 |    0.00 |      15.34 |           - |
| EntitiesWrite |      False |     8192 |   906.07 ms |   238.023 ms | 13.047 ms |  0.36 |    0.02 |     202.44 |           - |
| EntitiesWrite |       True |     8192 | 3,186.14 ms |   348.524 ms | 19.104 ms |  0.58 |    0.00 |      15.69 |           - |
| EntitiesWrite |      False |    16384 |   911.39 ms |   191.438 ms | 10.493 ms |  0.34 |    0.00 |     203.48 |           - |
| EntitiesWrite |       True |    16384 | 3,205.03 ms |   263.483 ms | 14.442 ms |  0.59 |    0.00 |      16.28 |           - |
|    TypedMerge |      False |     2048 |   657.70 ms |    15.185 ms |  0.832 ms |  0.24 |    0.00 |      47.09 |         7.7 |
|    TypedMerge |       True |     2048 | 1,074.05 ms |   286.135 ms | 15.684 ms |  0.19 |    0.00 |       6.58 |         1.8 |
|    TypedMerge |      False |     4096 |   663.72 ms |   180.549 ms |  9.896 ms |  0.25 |    0.01 |      46.75 |         7.7 |
|    TypedMerge |       True |     4096 | 1,069.50 ms |   101.372 ms |  5.557 ms |  0.19 |    0.00 |       6.67 |         1.8 |
|    TypedMerge |      False |     8192 |   660.07 ms |   105.579 ms |  5.787 ms |  0.26 |    0.01 |      47.24 |         7.8 |
|    TypedMerge |       True |     8192 | 1,042.57 ms |    44.117 ms |  2.418 ms |  0.19 |    0.00 |       6.99 |         1.8 |
|    TypedMerge |      False |    16384 |   653.96 ms |    79.998 ms |  4.385 ms |  0.25 |    0.00 |      48.07 |         7.8 |
|    TypedMerge |       True |    16384 | 1,053.15 ms |    95.668 ms |  5.244 ms |  0.19 |    0.00 |       7.31 |         1.9 |
|     TypedRead |      False |     2048 |    40.99 ms |     6.091 ms |  0.334 ms |  0.02 |    0.00 |     755.55 |         7.7 |
|     TypedRead |       True |     2048 |    61.37 ms |    15.728 ms |  0.862 ms |  0.01 |    0.00 |     115.08 |         1.8 |
|     TypedRead |      False |     4096 |    42.06 ms |    14.181 ms |  0.777 ms |  0.02 |    0.00 |     737.77 |         7.7 |
|     TypedRead |       True |     4096 |    61.36 ms |    25.174 ms |  1.380 ms |  0.01 |    0.00 |     116.25 |         1.8 |
|     TypedRead |      False |     8192 |    41.71 ms |     8.549 ms |  0.469 ms |  0.02 |    0.00 |     747.59 |         7.8 |
|     TypedRead |       True |     8192 |    58.36 ms |    28.889 ms |  1.584 ms |  0.01 |    0.00 |     124.77 |         1.8 |
|     TypedRead |      False |    16384 |    42.92 ms |    19.860 ms |  1.089 ms |  0.02 |    0.00 |     732.48 |         7.8 |
|     TypedRead |       True |    16384 |    57.98 ms |    34.715 ms |  1.903 ms |  0.01 |    0.00 |     132.86 |         1.9 |
|    TypedWrite |      False |     2048 |   504.20 ms |   186.807 ms | 10.240 ms |  0.19 |    0.00 |      61.42 |         7.7 |
|    TypedWrite |       True |     2048 |   830.32 ms |   126.500 ms |  6.934 ms |  0.15 |    0.00 |       8.51 |         1.8 |
|    TypedWrite |      False |     4096 |   502.38 ms |   130.062 ms |  7.129 ms |  0.19 |    0.01 |      61.77 |         7.7 |
|    TypedWrite |       True |     4096 |   820.94 ms |    28.988 ms |  1.589 ms |  0.15 |    0.00 |       8.69 |         1.8 |
|    TypedWrite |      False |     8192 |   491.59 ms |    50.117 ms |  2.747 ms |  0.19 |    0.01 |      63.43 |         7.8 |
|    TypedWrite |       True |     8192 |   817.13 ms |    42.363 ms |  2.322 ms |  0.15 |    0.00 |       8.91 |         1.8 |
|    TypedWrite |      False |    16384 |   498.27 ms |    50.761 ms |  2.782 ms |  0.19 |    0.00 |      63.09 |         7.8 |
|    TypedWrite |       True |    16384 |   821.35 ms |    64.661 ms |  3.544 ms |  0.15 |    0.00 |       9.38 |         1.9 |