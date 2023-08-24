import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DetailBaocaoChiComponent } from './detail-baocao-chi.component';

describe('DetailBaocaoChiComponent', () => {
  let component: DetailBaocaoChiComponent;
  let fixture: ComponentFixture<DetailBaocaoChiComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DetailBaocaoChiComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DetailBaocaoChiComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
