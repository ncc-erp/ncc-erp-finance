import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ChiChuyenDoiComponent } from './chi-chuyen-doi.component';

describe('ChiChuyenDoiComponent', () => {
  let component: ChiChuyenDoiComponent;
  let fixture: ComponentFixture<ChiChuyenDoiComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ChiChuyenDoiComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ChiChuyenDoiComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
