import { TestBed } from '@angular/core/testing';

import { CurrencyConvertService } from './currency-convert.service';

describe('CurrencyConvertService', () => {
  let service: CurrencyConvertService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CurrencyConvertService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
