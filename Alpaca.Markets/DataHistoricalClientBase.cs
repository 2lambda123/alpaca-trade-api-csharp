﻿using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Alpaca.Markets
{
    internal abstract class DataHistoricalClientBase<THistoricalBarsRequest, THistoricalQuotesRequest, THistoricalQuote, THistoricalTradesRequest>
        where THistoricalQuotesRequest : HistoricalRequestBase, Validation.IRequest
        where THistoricalTradesRequest : HistoricalRequestBase, Validation.IRequest
        where THistoricalBarsRequest : HistoricalRequestBase, Validation.IRequest
        where THistoricalQuote : IQuote, ISymbolMutable
    {
        protected readonly HttpClient HttpClient;

        protected DataHistoricalClientBase(
            HttpClient httpClient) =>
            HttpClient = httpClient;

        public void Dispose() => HttpClient.Dispose();

        public Task<IPage<IBar>> ListHistoricalBarsAsync(
            THistoricalBarsRequest request,
            CancellationToken cancellationToken = default) =>
            request.HasSingleSymbol
                ? listHistoricalBarsAsync(request, cancellationToken)
                : getHistoricalBarsAsync(request, cancellationToken).AsPageAsync<IBar, JsonBarsPage>();

        public Task<IMultiPage<IBar>> GetHistoricalBarsAsync(
            THistoricalBarsRequest request,
            CancellationToken cancellationToken = default) =>
            request.HasSingleSymbol
                ? listHistoricalBarsAsync(request, cancellationToken).AsMultiPageAsync<IBar, JsonMultiBarsPage>()
                : getHistoricalBarsAsync(request, cancellationToken);

        public Task<IPage<IQuote>> ListHistoricalQuotesAsync(
            THistoricalQuotesRequest request, 
            CancellationToken cancellationToken = default) =>
            request.HasSingleSymbol
                ? listHistoricalQuotesAsync(request, cancellationToken)
                : getHistoricalQuotesAsync(request, cancellationToken)
                    .AsPageAsync<IQuote, JsonQuotesPage<THistoricalQuote>>();

        public Task<IMultiPage<IQuote>> GetHistoricalQuotesAsync(
            THistoricalQuotesRequest request,
            CancellationToken cancellationToken = default) =>
            request.HasSingleSymbol
                ? listHistoricalQuotesAsync(request, cancellationToken)
                    .AsMultiPageAsync<IQuote, JsonMultiQuotesPage<THistoricalQuote>>()
                : getHistoricalQuotesAsync(request, cancellationToken);

        public Task<IPage<ITrade>> ListHistoricalTradesAsync(
            THistoricalTradesRequest request,
            CancellationToken cancellationToken = default) =>
            request.Symbols.Count == 1
                ? listHistoricalTradesAsync(request, cancellationToken)
                : getHistoricalTradesAsync(request, cancellationToken).AsPageAsync<ITrade, JsonTradesPage>();

        public Task<IMultiPage<ITrade>> GetHistoricalTradesAsync(
            THistoricalTradesRequest request, 
            CancellationToken cancellationToken = default) =>
            request.Symbols.Count == 1
                ? listHistoricalTradesAsync(request, cancellationToken).AsMultiPageAsync<ITrade, JsonMultiTradesPage>()
                : getHistoricalTradesAsync(request, cancellationToken);

        private async Task<IPage<IBar>> listHistoricalBarsAsync(
            THistoricalBarsRequest request,
            CancellationToken cancellationToken = default) =>
            await HttpClient.GetAsync<IPage<IBar>, JsonBarsPage>(
                await request.EnsureNotNull(nameof(request)).Validate()
                    .GetUriBuilderAsync(HttpClient).ConfigureAwait(false),
                cancellationToken).ConfigureAwait(false);

        private async Task<IMultiPage<IBar>> getHistoricalBarsAsync(
            THistoricalBarsRequest request,
            CancellationToken cancellationToken = default) =>
            await HttpClient.GetAsync<IMultiPage<IBar>, JsonMultiBarsPage>(
                await request.EnsureNotNull(nameof(request)).Validate()
                    .GetUriBuilderAsync(HttpClient).ConfigureAwait(false),
                cancellationToken).ConfigureAwait(false);

        private async Task<IPage<IQuote>> listHistoricalQuotesAsync(
            THistoricalQuotesRequest request, 
            CancellationToken cancellationToken = default) =>
            await HttpClient.GetAsync<IPage<IQuote>, JsonQuotesPage<THistoricalQuote>>(
                await request.EnsureNotNull(nameof(request)).Validate()
                    .GetUriBuilderAsync(HttpClient).ConfigureAwait(false),
                cancellationToken).ConfigureAwait(false);

        private async Task<IMultiPage<IQuote>> getHistoricalQuotesAsync(
            THistoricalQuotesRequest request, 
            CancellationToken cancellationToken = default) =>
            await HttpClient.GetAsync<IMultiPage<IQuote>, JsonMultiQuotesPage<THistoricalQuote>>(
                await request.EnsureNotNull(nameof(request)).Validate()
                    .GetUriBuilderAsync(HttpClient).ConfigureAwait(false),
                cancellationToken).ConfigureAwait(false);

        private async Task<IPage<ITrade>> listHistoricalTradesAsync(
            THistoricalTradesRequest request, 
            CancellationToken cancellationToken = default) =>
            await HttpClient.GetAsync<IPage<ITrade>, JsonTradesPage>(
                await request.EnsureNotNull(nameof(request)).Validate()
                    .GetUriBuilderAsync(HttpClient).ConfigureAwait(false),
                cancellationToken).ConfigureAwait(false);

        private async Task<IMultiPage<ITrade>> getHistoricalTradesAsync(
            THistoricalTradesRequest request, 
            CancellationToken cancellationToken = default) =>
            await HttpClient.GetAsync<IMultiPage<ITrade>, JsonMultiTradesPage>(
                await request.EnsureNotNull(nameof(request)).Validate()
                    .GetUriBuilderAsync(HttpClient).ConfigureAwait(false),
                cancellationToken).ConfigureAwait(false);
    }
}