import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DetailNhanvienNoComponent } from './detail-nhanvien-no.component';

describe('DetailNhanvienNoComponent', () => {
  let component: DetailNhanvienNoComponent;
  let fixture: ComponentFixture<DetailNhanvienNoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DetailNhanvienNoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DetailNhanvienNoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
