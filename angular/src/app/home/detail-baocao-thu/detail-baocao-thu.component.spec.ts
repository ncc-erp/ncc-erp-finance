import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DetailBaocaoThuComponent } from './detail-baocao-thu.component';

describe('DetailBaocaoThuComponent', () => {
  let component: DetailBaocaoThuComponent;
  let fixture: ComponentFixture<DetailBaocaoThuComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DetailBaocaoThuComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DetailBaocaoThuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
